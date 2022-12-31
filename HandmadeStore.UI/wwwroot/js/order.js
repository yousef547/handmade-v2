$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("pending")) {
        loadDataTable("pending");
    }
    else if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else if (url.includes("completed")) {
        loadDataTable("completed");
    }
    else {
        loadDataTable("all");
    }
});
let dataTable;
const loadDataTable = (status) => {
    dataTable = $('#dataTable').DataTable({
        "ajax": {
            "url": `/Admin/Order/GetAll?status=${status}`
        },
        columns: [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "25%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `    <div class="d-flex justify-content-around">
                                <a class="btn btn-warning " href="/Admin/Order/Details?orderId=${data}">
                                <i class="bi bi-pencil-square"></i></a>
                                                           </div>
                            `
                },
                width: "10%"
            }
        ],
        columnDefs: [
            {
                targets: 0,
                className: 'dt-left'
            }
        ]
    });
}

