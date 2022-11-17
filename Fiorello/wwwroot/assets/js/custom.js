jQuery(function ($) {
    $(document).on('click', '#addToCart', function () {
        var id = $(this).data('id');

        $.ajax({
            method: "POST",
            url: "/basket/add",
            data: {
                id: id
            },
            success: function () {
                console.log("ok");
            }
        })
    })

    $(document).on('click', '#deleteBtn', function () {
        var id = $(this).data('id');

        $.ajax({
            method: "POST",
            url: "/basket/delete",
            data: {
                id: id
            },
            success: function () {
                $(`.basket-product[id=${id}]`).remove();
            }
        })
    })
})