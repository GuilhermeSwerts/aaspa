export const GetParametro = (parametro) => {
    const params = new URLSearchParams(window.location.search);
    const param = params.get(parametro);
    return param;
}