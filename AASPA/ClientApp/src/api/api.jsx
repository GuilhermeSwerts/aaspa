import axios from "axios";

export default class Api {
    constructor(urlBase = "") {
        this.loginPage = "/login";
        this.urlBase = urlBase;

        this.access_token = window.localStorage.getItem("access_token");

        this.api = axios.create({
            baseURL: urlBase,
            headers: {
                "Authorization": `Bearer ${this.access_token ? this.access_token : ""}`
            }
        });
    }

    execute = (api, funcResult, funcError) => {

        if (window.Wait != undefined) window.Wait(true);
        document.getElementById("loadingpanel").style.display = 'flex';

        api.then((response) => {
            if (funcResult != undefined)
                funcResult(response);
            if (window.Wait != undefined)
                window.Wait(false);
            document.getElementById("loadingpanel").style.display = 'none';
        })
            .catch(async (err) => {
                debugger
                console.log({ error: err });
                if (err.response != null && err.response.status == 401) {
                    window.Wait(false);
                    window.localStorage.removeItem("access_token");
                    document.getElementById("loadingpanel").style.display = 'none';
                    // window.location.href = this.loginPage;
                    return;
                }
                if (funcError != undefined)
                    funcError(err);
                if (window.Wait != undefined)
                    window.Wait(false);
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

    uploadFileForm(endpoint, funcResult, funcError) {
        var input = document.querySelector('input[type="file"]')

        var data = new FormData()
        for (const file of input.files) {
            data.append('files', file, file.name)
        }

        this.post(endpoint, data, funcResult, funcError)
    }
}

export const api = new Api(process.env.NODE_ENV === 'production' ? 'http://hmlaaspa.aaspa.org.br' : "https://localhost:44326");