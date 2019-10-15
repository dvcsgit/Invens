var Application = {};
Application.loadAndShowDialog = function (id,link, url, width) {
    var separator = url.indexOf('?') >= 0 ? '&' : '?';
    var modalWidth = 650;
    if (width != undefined) modalWidth = width;
    $.get(url + separator)
            .done(function (content) {
                $("#myModal").empty();
                $("#myModal").html(content)
                    .filter('div') // Filter for the div tag only, script tags could surface
                    .modal('show')
                    .css({
                        width: modalWidth + 'px',
                        'margin-left': function () {
                            return -($(this).width() / 2);
                        }
                    });
                $.validator.unobtrusive.parse($("#myModal").find('form'));
            });
};