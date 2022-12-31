$(document).ready(function () {
    loadData();
});

let dataTable;
const loadData = () => {
    dataTable = $('#dataTable').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAllProducts"
        },
        columns: [
            { "data": "name", width: "25%" },
            { "data": "price", width: "10%" },
            { "data": "category.name", width: "20%" },
            { "data": "brand.name", width: "20%" },
            { "data": "createdDate", width: "10%" },

            {
                "data": "id",
                "render": function (data) {
                    return ` <a class="btn btn-warning mx-2"
                                href="/Admin/Product/Upsert?id=${data}"
                               ><i class="bi bi-pencil-square"></i></a>
                                <a class="btn btn-primary mx-2"
                                onclick=deleteProduct('/Admin/Product/Delete?id=${data}')
                               ><i class="bi bi-x-square"></i></a>`
                },
                width: "10%"
            },

        ],
        columnDefs: [
            {
                targets: 4,
                render: DataTable.render.date()
            }],
        dom: 'Bfrtip',
        responsive: true,
        lengthChange: false,
        autoWidth: false,
        buttons: ["copy",
            {
                extend: 'excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                }
            },
            {
                extend: 'pdf',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                }
            }]

    });
    dataTable.buttons().container().prependTo("#dataTable_wrapper");

}

const deleteProduct = (url) => {
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