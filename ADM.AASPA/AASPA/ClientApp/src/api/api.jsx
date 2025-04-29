import axios from "axios";

// const url = process.env.ENVIRONMENT == 'production' ? 'https://adm.aaspa.org.br' : 'https://adm.aaspa.org.br';
const url = 'https://localhost:5001';

export default class Api {
    constructor(urlBase = "") {
        this.loginPage = "/login";
        this.urlBase = urlBase;

        this.access_token = window.localStorage.getItem("access_token");
         this.ambiente = url;

        this.api = axios.create({
            baseURL: urlBase,
            headers: {
                "Authorization": `Bearer ${this.access_token ? this.access_token : ""}`
            }
        });
    }

    execute = (api, funcResult, funcError) => {

        if (window.Wait != undefined) window.Wait(true);

        if (document.getElementById("loadingpanel"))
            document.getElementById("loadingpanel").style.display = 'flex';

        api.then((response) => {
            if (funcResult != undefined)
                funcResult(response);
            if (window.Wait != undefined)
                window.Wait(false);

            if (document.getElementById("loadingpanel"))
                document.getElementById("loadingpanel").style.display = 'none';
        })
            .catch(async (err) => {
                console.log({ error: err });
                if (err.response != null && err.response.status == 401) {
                    window.Wait && window.Wait(false);
                    window.localStorage.removeItem("access_token");
                    document.getElementById("loadingpanel").style.display = 'none';
                    window.location.href = this.loginPage;
                    return;
                }
                if (funcError != undefined)
                    funcError(err);
                if (window.Wait != undefined)
                    window.Wait(false);

                if (document.getElementById("loadingpanel"))
                    document.getElementById("loadingpanel").style.display = 'none';
            });

    }

    get = (url, funcResult, funcError) => {
        this.execute(this.api.get(url), funcResult, funcError);
    }

    getForm = (url, form, funcResult, funcError) => {
        if (url.indexOf('?') <= 0) url += "?";
        var first = true;
        for (var c in form) {
            if (!first) url += "&";
            url += c + "=" + form[c];
            first = false;
        }

        this.execute(this.api.get(url), funcResult, funcError);
    }

    delete = (url, funcResult, funcError) => {
        this.execute(this.api.delete(url), funcResult, funcError);
    }

    post = (url, form, funcResult, funcError) => {
        this.execute(this.api.post(url, form), funcResult, funcError);
    }

    put = (url, form, funcResult, funcError) => {
        this.execute(this.api.put(url, form), funcResult, funcError);
    }

    ArquviGrande = async (endpoint, funcResult, funcError, setProgress = () => { }) => {
        var input = document.querySelector('input[type="file"]');

        if (!input.files.length) {
            console.error("Nenhum arquivo selecionado.");
            return;
        }
        const file = input.files[0];
        const formData = new FormData();
        formData.append("file", file);
        
        await axios.post(endpoint, formData, {
            baseURL: this.urlBase,
            headers: {
                "Content-Type": "multipart/form-data",
                "Authorization": `Bearer ${this.access_token ?? ""}`
            },
            onUploadProgress: (progressEvent) => {
                const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total);
                setProgress(percent);
            },
        }).then(response => funcResult(response))
            .catch(erro => funcError(erro));
    };


    uploadFileForm(endpoint, funcResult, funcError) {
        var input = document.querySelector('input[type="file"]')

        var data = new FormData()
        for (const file of input.files) {
            data.append('files', file, file.name)
        }

        this.post(endpoint, data, funcResult, funcError)
    }
}

export const api = new Api(url);
