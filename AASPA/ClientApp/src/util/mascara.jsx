export const Mascara = {
    cnpj: function cnpj(v) {
        v = v.replace(/\D/g, "")
        v = v.replace(/^(\d{2})(\d)/, "$1.$2")
        v = v.replace(/^(\d{2})\.(\d{3})(\d)/, "$1.$2.$3")
        v = v.replace(/\.(\d{3})(\d)/, ".$1/$2")
        v = v.replace(/(\d{4})(\d)/, "$1-$2")
        return v
    },
    cpf: function cpf(v) {
        v = v.replace(/\D/g, "")
        v = v.replace(/(\d{3})(\d)/, "$1.$2")
        v = v.replace(/(\d{3})(\d)/, "$1.$2")
        v = v.replace(/(\d{3})(\d{1,2})$/, "$1-$2")
        return v
    },
    cep: function cep(v) {
        v = v.replace(/\D/g, "")
        v = v.replace(/(\d{5})(\d)/, "$1-$2")
        return v
    },
    cpfOrCnpj: function cpfOrCnpj(v) {
        if (!v) return;
        if (v.length > 11) {
            v = this.cnpj(v);
        } else {
            v = this.cpf(v);
        }
        debugger
        return v;
    },
    rgOrCnh: function rgOrCnh(v) {
        v = v.replace(/\D/g, "")
        if (v.length < 9) {
            v = v.replace(/(\d{10})(\d)/, "$1$2")
            return v
        } else {
            v = v.replace(/(\d{3})(\d)/, "$1.$2")
            v = v.replace(/(\d{3})(\d)/, "$1.$2")
            v = v.replace(/(\d{3})(\d{1,2})$/, "$1-$2")
            return v
        }
    },
    telefone: function telefone(v) {
        if (!v) return;

        v = v.replace(/\D/g, "")
        v = v.replace(/(\d{1})(\d)/, "($1$2")
        v = v.replace(/(\d{2})(\d)/, "$1) $2")
        v = v.replace(/(\d{5})(\d)/, "$1-$2")
        return v
    },
    telefoneFixo: function telefoneFixo(v) {
        if (!v) return;
        v = v.replace(/\D/g, "")
        v = v.replace(/(\d{1})(\d)/, "($1$2")
        v = v.replace(/(\d{2})(\d)/, "$1) $2")
        v = v.replace(/(\d{4})(\d)/, "$1-$2")
        return v
    },
    moeda: function moeda(v) {
        if (!v) return;
        return v.replace(/[^\d,]/g, "")
            .replace(/\./g, ",")
            .replace(/(\d)(?=(\d{0})+(?!\d))/g, '$1')
            .replace(/(\d{1})(\d)/, "R$ $1$2")

    },
    data: function data(dateString) {
        const date = new Date(dateString);

        const options = {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: false
        };

        const formattedDate = date.toLocaleString('pt-BR', options);

        return formattedDate.replace(/,/, '');
    },
    dataSemHora: function data(dataStr) {
        const data = new Date(dataStr);

        const dia = data.getDate() + 1;
        const mes = data.getMonth() + 1;
        const ano = data.getFullYear();

        const diaFormatado = dia.toString().padStart(2, '0');
        const mesFormatado = mes.toString().padStart(2, '0');

        return `${diaFormatado}/${mesFormatado}/${ano}`;
    }

}