var commentLoginInfo = false;
$(function () {
    $('#comment').keyup(function () {
        if ($(this).val().length > 0) { if (!commentLoginInfo) { commentLoginInfo = true; $('#signInInfo').show(700); } }
        else { commentLoginInfo = false; $('#signInInfo').hide(700); }
    });

    $('.replyToComment').click(function () {
        if ($(this).text() == ' Cancel') {
            $(this).html('<i class="fa fa-reply"></i> Reply');
            $("#commentForm").insertAfter($('.commentHeading'));
            $("#parentCommentId").val('');
        } else {
            $(this).html('<i class="fa fa-close"></i> Cancel');
            var parentCommentId = $(this).data('replyto');
            $("#parentCommentId").val(parentCommentId);
            $('.replyToComment-' + parentCommentId + ' p').append($('#commentForm'));
        }
    });

    $('#commentForm').submit(function (e) {
        var formValid = true;
        if (formValid && $('#comment').val().length < 10) { formValid = false; alert('Please enter few words to make it look good!'); }
        if (formValid && $('#email').val().length < 6) { formValid = false; alert('Please enter valid email'); }
        if (formValid && $('#name').val().length < 2) { formValid = false; alert('Please enter your name'); }
        if (formValid) {
            $.post('/Post/AddComment/@Model.Id', $(this).serialize())
                .done(function () { $('#signInInfo').hide(200); $('#comment').hide(500).val('').show(500); })
                .fail(function () { $('#comment').hide(200).show(300); });
        }
        e.preventDefault();
    });
});
