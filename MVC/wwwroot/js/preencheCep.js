	$("#cep").focusout(function(){
		$.ajax({
			url: 'https://viacep.com.br/ws/' + $(this).val() + '/json/',
			dataType: 'json',
			success: function (resposta) {
				$("#logradouro").val(resposta.logradouro);
				$("#complemento").val(resposta.complemento);
				$("#cidade").val(resposta.localidade);
				$("#bairro").val(resposta.bairro);
                $("#estado").val(() => {                
                    if (resposta.uf == "AC")
                        return "Acre";
                    if (resposta.uf == "AL")
                        return "Alagoas";
                    if (resposta.uf == "AP")
                        return "Amapá";
                    if (resposta.uf == "AM")
                        return "Amazonas";
                    if (resposta.uf == "BA")
                        return "Bahia";
                    if (resposta.uf == "CE")
                        return "Ceará";
                    if (resposta.uf == "DF")
                        return "Distrito Federal";
                    if (resposta.uf == "ES")
                        return "Espírito Santo";
                    if (resposta.uf == "GO")
                        return "Goiás";
                    if (resposta.uf == "MA")
                        return "Maranhão";
                    if (resposta.uf == "MT")
                        return "Mato Grosso";
                    if (resposta.uf == "MS")
                        return "Mato Grosso do Sul";
                    if (resposta.uf == "MG")
                        return "Minas Gerais";
                    if (resposta.uf == "PA")
                        return "Pará";
                    if (resposta.uf == "PB")
                        return "Paraíba";
                    if (resposta.uf == "PR")
                        return "Paraná";
                    if (resposta.uf == "PE")
                        return "Pernambuco";
                    if (resposta.uf == "PI")
                        return "Piauí";
                    if (resposta.uf == "RJ")
                        return "Rio de Janeiro";
                    if (resposta.uf == "RN")
                        return "Rio Grande do Norte";
                    if (resposta.uf == "RS")
                        return "Rio Grande do Sul";
                    if (resposta.uf == "RO")
                        return "Rondônia";
                    if (resposta.uf == "RR")
                        return "Roraima";
                    if (resposta.uf == "SC")
                        return "Santa Catarina";
                    if (resposta.uf == "SP")
                        return "São Paulo";
                    if (resposta.uf == "SE")
                        return "Sergipe";
                    if (resposta.uf == "TO")
                        return "Tocantins";                    
				});
				$("#uf").val(resposta.uf);
				$("#numero").focus();
			}
		});
    });

function Tipo(seletor, tipoMascara) {
    setTimeout(function () {
        if (tipoMascara == 'CPFCNPJ') {
            if (seletor.value.length <= 14) { //cpf
                formataCampo(seletor, '000.000.000-00');
            } else { //cnpj
                formataCampo(seletor, '00.000.000/0000-00');
            }
        } else if (tipoMascara == 'DATA') {
            formataCampo(seletor, '00/00/0000');
        } else if (tipoMascara == 'CEP') {
            formataCampo(seletor, '00000-000');
        } else if (tipoMascara == 'TELEFONE') {
            formataCampo(seletor, '(00) 000000000');
        } else if (tipoMascara == 'CPF') {
            formataCampo(seletor, '000.000.000-00');
        } else if (tipoMascara == 'CNPJ') {
            formataCampo(seletor, '00.000.000/0000-00');
        } else if (tipoMascara == 'MOEDA') {
            MascaraMoeda(seletor)
        }
    }, 200);
}

function formataCampo(campo, Mascara) {
    var er = /[^0-9/ (),.-]/;
    er.lastIndex = 0;

    if (er.test(campo.value)) {///verifica se é string, caso seja então apaga
        var texto = $(campo).val();
        $(campo).val(texto.substring(0, texto.length - 1));
    }
    var boleanoMascara;
    var exp = /\-|\.|\/|\(|\)| /g
    var campoSoNumeros = campo.value.toString().replace(exp, "");
    var posicaoCampo = 0;
    var NovoValorCampo = "";
    var TamanhoMascara = campoSoNumeros.length;
    for (var i = 0; i <= TamanhoMascara; i++) {
        boleanoMascara = ((Mascara.charAt(i) == "-") || (Mascara.charAt(i) == ".")
                || (Mascara.charAt(i) == "/"))
        boleanoMascara = boleanoMascara || ((Mascara.charAt(i) == "(")
                || (Mascara.charAt(i) == ")") || (Mascara.charAt(i) == " "))
        if (boleanoMascara) {
            NovoValorCampo += Mascara.charAt(i);
            TamanhoMascara++;
        } else {
            NovoValorCampo += campoSoNumeros.charAt(posicaoCampo);
            posicaoCampo++;
        }
    }
    campo.value = NovoValorCampo;
    if (campo.value.length > Mascara.length) {
        var texto = $(campo).val();
        $(campo).val(texto.substring(0, texto.length - 1));
    }
    return true;
}

function MascaraMoeda(i) {
    var v = i.value.replace(/\D/g, '');
    v = (v / 100).toFixed(2) + '';
    v = v.replace(".", ",");
    v = v.replace(/(\d)(\d{3})(\d{3}),/g, "$1.$2.$3,");
    v = v.replace(/(\d)(\d{3}),/g, "$1.$2,");
    i.value = v;
}