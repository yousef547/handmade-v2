using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using HandmadeStore.Models.Models;
using HandmadeStore.Models.Models.ViewModels;
using HandmadeStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;

namespace HandmadeStore.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        private CartVM cartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            this._emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            cartVM = new CartVM()
            {
                CartItems = _unitOfWork.CartItem.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product")
            };
            foreach (var item in cartVM.CartItems)
            {
                item.Price = GetPrice(item.Count, item.Product.Price, item.Product.Price10Plus, item.Product.Price30Plus);
                cartVM.CartTotle += (item.Price * item.Count);
                cartVM.PriceCount += item.Count;

            }
            return View(cartVM);
        }



        public IActionResult Increment(int cartId)
        {
            var cart = _unitOfWork.CartItem.GetFirstOrDefault(x => x.Id == cartId);
            cart.Count = cart.Count + 1;
            _unitOfWork.CartItem.Update(cart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.CartSession, _unitOfWork.CartItem.GetPiecsCount());
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Decrement(int cartId)
        {
            var cart = _unitOfWork.CartItem.GetFirstOrDefault(x => x.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.CartItem.Remove(cart);
            }
            else
            {
                cart.Count = cart.Count - 1;
                _unitOfWork.CartItem.Update(cart);
            }
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.CartSession, _unitOfWork.CartItem.GetPiecsCount());
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int cartId)
        {
            var cart = _unitOfWork.CartItem.GetFirstOrDefault(x => x.Id == cartId);
            if (cart == null)
            {
                return NotFound();
            }
            _unitOfWork.CartItem.Remove(cart);
            _unitOfWork.Save(); HttpContext.Session.SetInt32(SD.CartSession, _unitOfWork.CartItem.GetPiecsCount());
            return RedirectToAction(nameof(Index));

        }
        private double GetPrice(int count, double? price, double? price10plus, double? price30plus)
        {
            if (count <= 10)
            {
                return (double)price;
            }
            else
            {
                if (count <= 30)
                {
                    return (double)price10plus;
                }
                else
                {
                    return (double)price30plus;
                }
            }
        }

        public IActionResult Summery()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            cartVM = new CartVM()
            {
                CartItems = _unitOfWork.CartItem.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()

            };
            cartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == userId);
            cartVM.OrderHeader.Name = cartVM.OrderHeader.ApplicationUser.Name;
            cartVM.OrderHeader.PhoneNumber = cartVM.OrderHeader.ApplicationUser.PhoneNumber;
            cartVM.OrderHeader.StreetAddress = cartVM.OrderHeader.ApplicationUser.StreetAdress;
            cartVM.OrderHeader.City = cartVM.OrderHeader.ApplicationUser.City;
            cartVM.OrderHeader.PostalCode = cartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var item in cartVM.CartItems)
            {
                item.Price = GetPrice(item.Count, item.Product.Price, item.Product.Price10Plus, item.Product.Price30Plus);
                cartVM.CartTotle += (item.Price * item.Count);
                cartVM.OrderHeader.OrderTotal += (item.Price * item.Count);
                cartVM.PriceCount += item.Count;
            }
            return View(cartVM);
        }

        [HttpPost]
        [ActionName("Summery")]
        public IActionResult SummeryPost(CartVM cartVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            cartVM.CartItems = _unitOfWork.CartItem.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
            if (applicationUser.ShopId.GetValueOrDefault() == 0)
            {
                cartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                cartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                cartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                cartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            cartVM.OrderHeader.OrderDate = DateTime.Now;
            cartVM.OrderHeader.ApplicationUserId = userId;
            foreach (var item in cartVM.CartItems)
            {
                item.Price = GetPrice(item.Count, item.Product.Price, item.Product.Price10Plus, item.Product.Price30Plus);
                cartVM.CartTotle += (item.Price * item.Count);
                cartVM.OrderHeader.OrderTotal += (item.Price * item.Count);
                cartVM.PriceCount += item.Count;
            }
            _unitOfWork.OrderHeader.Add(cartVM.OrderHeader);
            _unitOfWork.Save();


            foreach (var item in cartVM.CartItems)
            {
                OrderDetail orderDetails = new()
                {
                    ProductId = item.ProductId,
                    OrderId = cartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetails);
                _unitOfWork.Save();
            }
            if (applicationUser.ShopId.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:44352/";
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>(),

                    Mode = "payment",
                    PaymentMethodTypes = new List<string>()
                {
                    "card",
                },
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={cartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index"
                };

                foreach (var item in cartVM.CartItems)
                {
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Price * 100),
                                Currency = "egp",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Name,
                                },
                            },
                            Quantity = item.Count,
                        };
                        options.LineItems.Add(sessionLineItem);
                    }
                }

                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateOrderPayment(cartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                return RedirectToAction("OrderConfirmation", "cart", new { id = cartVM.OrderHeader.Id });
            }


            //_unitOfWork.Save();
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "Paid")
                {
                    _unitOfWork.OrderHeader.UpdateOrderPayment(orderHeader.Id, session.Id, session.PaymentIntentId);

                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Handmade Store", $"<h2>Order #{orderHeader.Id} Created Successfully ... Total : {orderHeader.OrderTotal} EGP</h2>");
            List<CartItem> cartItems = _unitOfWork.CartItem.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.CartItem.RemoveRange(cartItems);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.CartSession,0);
            return View(id);
        }

    }
}




