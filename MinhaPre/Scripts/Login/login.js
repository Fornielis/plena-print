$(function () {
    // AVISO LOGIN
    $('#erro-login .btn').on('click', function () {
        $("#erro-login").removeClass("zoomInDown");
        $("#erro-login").addClass("bounceOut");
        $("input[name='userName']").val('');
        $("input[name='Senha']").val('');
    });
});