//MASCARAS CAMPOS TEXTOS
function mascarasTextos() {
    $(".mask-prazo-data").mask("99/99/99");
    $(".mask-prazo-hora").mask("99:99");
};

//REQUISIÇÃO ASSINCRONA
//LINHA DA TABELA FUNCIONA COMO LINK
function linkLinha(url) {
    abrirTelaProcess();
    $('html, body').animate({ scrollTop: 0 }, 'slow');
    $.get(url, function (resultado) {
        $("#retorno").html(resultado);
    })
};

// ABRIR - FECHAR TELA PROCESS
function abrirTelaProcess() {
    $("#loader").css('display', 'block');
};

function fecharTelaProcess() {
    $("#loader").css('display', 'none');
};

function camposProvaImpressoras() {
    $("#camposProvaImpressoras").css('display', 'block');
    $("#BtnAlterarProvasImpressoras").css('display', 'none');
};

function FecharJanelaAviso() {
    $("#BoxAviso").css("display", "none");
};

function AbrirJanelaAviso() {
    $("#BoxAviso").css("display", "block");
};

function FecharChapaGravada() {
    $("#ChapasGravadas").css("display", "none");
    document.getElementById("retorno").innerHTML = "";
};


