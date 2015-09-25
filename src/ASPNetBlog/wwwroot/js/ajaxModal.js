
$(function () {
    //Optional: turn the chache off
    $.ajaxSetup({ cache: false });
    $('#loginWith a').click(function () {
        $('#dialogContent').load(this.href, function () {
            $('#ajaxModal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            bindForm(this);
        });
        return false;
    });
});

function bindForm(dialog) {
    $('#loginForm', dialog).submit(function () {
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    $('#ajaxModal').modal('hide');
                    $('#email').val(result.email).attr('readonly', 'readonly');
                    $('#name').val(result.name).attr('readonly', 'readonly');
                    $('#loginWith').hide(1000);
                } else {
                    $('#dialogContent').html(result);
                    bindForm();
                }
            }
        });
        return false;
    });
}