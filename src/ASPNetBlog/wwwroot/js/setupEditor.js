(function ($) {
    $.fn.setupEditor = function () {
        var $this = this;

        $this.on('summernote.init', function() { 
                // Add "Pre" code button
                var noteBtn = '<button id="makeCode" type="button" class="btn btn-default btn-sm btn-small" data-tooltip data-original-title="Identify a source code" data-event="something" tabindex="-1"><i class="fa fa-file-code-o"></i></button>';
                var fileGroup = '<div class="note-file btn-group">' + noteBtn + '</div>';
                $(fileGroup).appendTo($('.note-toolbar'));
                // Button tooltips
                $('#makeCode').tooltip({ container: 'body', placement: 'bottom' });
                // Button events
                $('#makeCode').click(function (event) {
                    var highlight = window.getSelection(),
                        pre = document.createElement('pre'),
                        range = highlight.getRangeAt(0)

                    pre.innerHTML = highlight;

                    range.deleteContents();
                    range.insertNode(pre);
                });
            })
            .summernote({
            focus: false,
            height: 320,
            codemirror: {
                theme: 'united'
            },
            prettifyHtml: false,
            oninit: function () {
            },
        });
    }
}(jQuery));