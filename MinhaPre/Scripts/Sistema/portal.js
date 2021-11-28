// FUNÇÃO QUE CARREGA CONTROLER QUE DETERMINA OS DADOS INICIAIS
function carregamentoInicialDados() {
    $("#loader").css('display', 'block');
    $.get('/Sistema/DadosIniciais', function (resultado) {
        $('#dados').html(resultado);
    });
};

function FecharJanelaErro() {
    $("#Erro").css("display", "none");
};

function FecharJanelaRetorno() {
    $("#Retorno").css("display", "none");
};

function Fechatudo() {
    var telaBloqueio = document.getElementById('telaBloqueio');
    telaBloqueio.style.display = "none";

    var menu = document.getElementById('menu');
    menu.style.left = "-300px";

    var dadosUsuario = document.getElementById('dadosUsuario');
    dadosUsuario.style.right = "-300px";
}

function FecharLoader() {
    $("#loader").css('display', 'none');
}

function LimparRetorno() {
    document.getElementById("retorno").innerHTML = "";
}

$('#telaBloqueio').on('click', Fechatudo);




