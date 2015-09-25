$(function () {
    $('.btn-social-share').each(function () {
        $(this).attr('href', $(this).attr('href').replace('[URL]', $(location).attr('href')).replace('[TITLE]', document.title)).attr('target','_blank');
    });
});