(function ($) {
    $.fn.showCharacterCount = function () {
        this.each(function () { addCharacterCount(this); })
            .bind('keyup', function () {
                var $element = $(this),
                    maxLength = $element.data('val-length-max');
                if ($element.val().length > maxLength)
                    $element.val($element.val().substring(0, maxLength));
                $("#" + this.id + "-counter").text(maxLength - $element.val().length);
            });
        return this;
    };

    function addCharacterCount(input) {
        var $input = $(input),
            maxLength = $input.data('val-length-max'),
            remaining = maxLength - $input.val().length;
        $('<span class="label label-warning"><span id="' + input.id + '-counter">' + remaining + '</span> remaining (max ' + maxLength + ')</span>').insertAfter(input);
    }

}(jQuery));



