$(document).ready(function () {
    loadData()
});
let dataTable;
const loadData = () => {
    dataTable = $('#dataTable').DataTable({
        "ajax": {
            "url": "/Admin/Shop/GetAll"
        },
        columns: [
            { "data": "name", width: "25%" },
            { "data": "city", width: "10%" },
            { "data": "streetAddress", width: "20%" },
            { "data": "postalCode", width: "15%" },
            { "data": "phoneNumber", width: "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `    <div class="d-flex justify-content-around">
                                <a class="btn btn-warning " href="/Admin/Shop/Upsert?id=${data}">
                                <i class="bi bi-pencil-square"></i></a>
                                <a onClick=deleteShop('/Admin/Shop/Delete/${data}') class="btn btn-danger " ><i class="bi bi-x-square"></i></a>
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

const deleteShop = (url) => {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}