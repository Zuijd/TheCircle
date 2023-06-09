$(document).ready(function () {
    $('#startStreamBtn').click(function () {
        var button = $(this);
        var videoBox = $('#videoBox');

        if (button.hasClass('btn-success')) {
            button.removeClass('btn-success').addClass('btn-danger').text('Stop Streaming');
            videoBox.show();
        } else {
            button.removeClass('btn-danger').addClass('btn-success').text('Start Streaming');
            videoBox.hide();
        }
    });
});
