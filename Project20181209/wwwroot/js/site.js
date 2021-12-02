// Write your JavaScript code.

$('.item-english').click(function (elemt) {
    $('.item-english').each(function (index, item) {
        item.className = "item-english";
    })
    elemt.target.className = "item-english warning";
    // var id = $(elemt.target).data('englishid');
    // $(elemt.target.parentElement.parentElement.parentElement.parentElement.nextElementSibling).find('englishId');
})

$('.myModal').on('hidden.bs.modal', function () {
    $('.item-english').each(function (index, item) {
        item.className = "item-english";
    })
    $('.englishId').val();
})

$('.confirmModal').on('hidden.bs.modal', function () {
    $(".start_date").val("");
    $(".end_date").val("");
    $(".end_date").attr("disabled", "disabled");
    $(".end_date").removeAttr("min");
})


// UserDetail - Confirm
$(".start_date").change(function () {
    if (this.value != null) {
        $(".end_date").val("");
        $(".end_date").attr("min", this.value);
        $(".end_date").removeAttr("disabled");
    }
})