


function Avaliar(estrela) {
    var url = window.location;
    url = url.toString()
    url = url.split("Privacy");
    url = url[0];

    var s1 = document.getElementById("s1").src;
    var s2 = document.getElementById("s2").src;
    var s3 = document.getElementById("s3").src;
    var s4 = document.getElementById("s4").src;
    var s5 = document.getElementById("s5").src;
    var avaliacao = 0;

    if (estrela == 5) {
        if (s5 == url + "../icones/star.png") {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star1.png";
            document.getElementById("s3").src = "../icones/star1.png";
            document.getElementById("s4").src = "../icones/star1.png";
            document.getElementById("s5").src = "../icones/star.png";
            avaliacao = 4;
        } else {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star1.png";
            document.getElementById("s3").src = "../icones/star1.png";
            document.getElementById("s4").src = "../icones/star1.png";
            document.getElementById("s5").src = "../icones/star1.png";
            avaliacao = 5;
        }
    }

    //ESTRELA 4                                             
    if (estrela == 4) {
        if (s4 == url + "../icones/star.png") {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star1.png";
            document.getElementById("s3").src = "../icones/star1.png";
            document.getElementById("s4").src = "../icones/star1.png";
            document.getElementById("s5").src = "../icones/star1.png";
            avaliacao = 5;
        } else {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star1.png";
            document.getElementById("s3").src = "../icones/star1.png";
            document.getElementById("s4").src = "../icones/star1.png";
            document.getElementById("s5").src = "../icones/star.png";
            avaliacao = 4;
        }
    }

    //ESTRELA 3                                             
    if (estrela == 3) {
        if (s3 == url + "../icones/star.png") {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star1.png";
            document.getElementById("s3").src = "../icones/star.png";
            document.getElementById("s4").src = "../icones/star.png";
            document.getElementById("s5").src = "../icones/star.png";
            avaliacao = 2;
        } else {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star1.png";
            document.getElementById("s3").src = "../icones/star1.png";
            document.getElementById("s4").src = "../icones/star.png";
            document.getElementById("s5").src = "../icones/star.png";
            avaliacao = 3;
        }
    }

    //ESTRELA 2                                              
    if (estrela == 2) {
        if (s2 == url + "../icones/star.png") {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star.png";
            document.getElementById("s3").src = "../icones/star.png";
            document.getElementById("s4").src = "../icones/star.png";
            document.getElementById("s5").src = "../icones/star.png";
            avaliacao = 1;
        } else {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star1.png";
            document.getElementById("s3").src = "../icones/star.png";
            document.getElementById("s4").src = "../icones/star.png";
            document.getElementById("s5").src = "../icones/star.png";
            avaliacao = 2;
        }
    }

    //ESTRELA 1
    if (estrela == 1) {
        if (s1 == url + "../icones/star.png") {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star.png";
            document.getElementById("s3").src = "../icones/star.png";
            document.getElementById("s4").src = "../icones/star.png";
            document.getElementById("s5").src = "../icones/star.png";
            avaliacao = 0;
        } else {
            document.getElementById("s1").src = "../icones/star1.png";
            document.getElementById("s2").src = "../icones/star.png";
            document.getElementById("s3").src = "../icones/star.png";
            document.getElementById("s4").src = "../icones/star.png";
            document.getElementById("s5").src = "../icones/star.png";
            avaliacao = 1;
        }

    }

    document.getElementById('rating').innerHTML = avaliacao;
}

// Produtos

function img1() {
    document.getElementById('img').src = '../img/galaxia/galaxia1.jpg';

}
function img2() {
    document.getElementById('img').src = '../img/galaxia/galaxia2.jpg';
}
function img3() {
    document.getElementById('img').src = '../img/galaxia/galaxia3.jpg';
}

//Função olho
function mostrar() {
    document.getElementById('olho').src = '../icones/eye.svg';
    document.getElementById('password').type = 'password';
}
function ocultar() {
    document.getElementById('olho').src = '../icones/eye-off.svg';
    document.getElementById('password').type = 'text';
}

// Criar Contas
function mostrarSenha() {
    document.getElementById('olhoSenha').src = '../icones/eye.svg';
    document.getElementById('passwordCriar').type = 'password';
}
function ocultarSenha() {
    document.getElementById('olhoSenha').src = '../icones/eye-off.svg';
    document.getElementById('passwordCriar').type = 'text';
}
function mostrarConf() {
    document.getElementById('olhoConf').src = '../icones/eye.svg';
    document.getElementById('passwordConf').type = 'password';
}
function ocultarConf() {
    document.getElementById('olhoConf').src = '../icones/eye-off.svg';
    document.getElementById('passwordConf').type = 'text';
}

function ValidaCriaConta() {
    var ObjNome = document.getElementById("nome");
    var ObjEmail = document.getElementById("email");
    var ObjSenha = document.getElementById("senha");
    if (!ObjNome.checkValidity()) {
        document.getElementById("validNome")
            .innerHTML = inpObj.validationMessage;
    }
    if (!ObjEmail.checkValidity()) {
        document.getElementById("validEmail")
            .innerHTML = ObjEmail.validationMessage;
    }
    if (!ObjSenha.checkValidity()) {
        document.getElementById("validSenha")
            .innerHTML = ObjSenha.validationMessage;
    }


}