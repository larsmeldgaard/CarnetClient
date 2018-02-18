using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace CarnetClient
{
    public partial class CarNet
    {
        const string auth_base = "https://security.volkswagen.com";
        const string carnetbase = "https://www.volkswagen-car-net.com";

        string CRSF_TOKEN = "";

        IRestClient commandClient = new RestClient(carnetbase);
        public bool Carnet_Logon(string UserName, string Password)
        {

            CookieContainer _cookieJar = new CookieContainer();
            var client = new RestClient(carnetbase);
            client.CookieContainer = _cookieJar;
            Console.WriteLine("Getting csrf-token");
            var request = new RestRequest("/portal/en_GB/web/guest/home");
            var resp = client.Execute(request);
            var csrf = extract_csrf(resp.Content);

            Console.WriteLine("Getting login and viewstate");
            request.Resource = "/portal/web/guest/home/-/csrftokenhandling/get-login-url";
            var resp2 = client.ExecuteAsPost(request, "POST");
            dynamic resp2json = JsonConvert.DeserializeObject<dynamic>(resp2.Content);
            request.Resource = resp2json.loginURL.path;
            var ex_resp3 = client.Execute(request);
            string viewstate = extract_view_state(ex_resp3.Content);

            Console.WriteLine("Logging in");
            client.BaseUrl = new System.Uri(auth_base);
            var loginRequest = new RestRequest(Method.POST);
            loginRequest.AddParameter("loginForm", "loginForm");
            loginRequest.AddParameter("loginForm:email", UserName);
            loginRequest.AddParameter("loginForm:password", Password);
            loginRequest.AddParameter("javax.faces.ViewState", viewstate);
            loginRequest.AddParameter("javax.faces.source", "loginForm:submit");
            loginRequest.AddParameter("javax.faces.partial.event", "click");
            loginRequest.AddParameter("javax.faces.partial.execute", "loginForm:submit loginForm");
            loginRequest.Resource = "/ap-login/jsf/login.jsf";
            loginRequest.RequestFormat = DataFormat.Json;
            loginRequest.AddHeader("Faces-Request", "partial/ajax");

            var loginResponse = client.Execute(loginRequest);
            var redirect_url = extract_redirect_url(loginResponse.Content).Replace("&amp;", "&");

            Console.WriteLine("Extracting code, preparing for new redirect");
            client.BaseUrl = new System.Uri(redirect_url);
            client.FollowRedirects = false; //Because we need to modify this URL and not just follow the location
            request.Resource = "";
            var resp5 = client.Execute(request);

            var ref_url2 = resp5.Headers.Where(x => x.Name == "Location").FirstOrDefault().Value.ToString();
            var parsed = HttpUtility.ParseQueryString(new System.Uri(ref_url2).Query);

            var code = parsed["code"];
            var state = parsed["state"];
            var ref_url2_Path = new System.Uri(ref_url2).AbsolutePath;

            request.AddParameter("_33_WAR_cored5portlet_code", code);
            client.BaseUrl = new System.Uri(carnetbase);
            client.FollowRedirects = true;
            Console.WriteLine("Final login");
            request.Resource = ref_url2_Path + "?p_auth=" + state + "&p_p_id=33_WAR_cored5portlet&p_p_lifecycle=1&p_p_state=normal&_33_WAR_cored5portlet_javax.portlet.action=getLoginStatus";

            var FinalLoginRequest = client.Execute(request);

            CRSF_TOKEN = extract_csrf(FinalLoginRequest.Content);
            commandClient.BaseUrl = FinalLoginRequest.ResponseUri;
            commandClient.CookieContainer = _cookieJar;
            Console.WriteLine("Login Successful");
            return true;
        }

        public string carnet_post(string action)
        {
            Console.WriteLine(action);

            var request = new RestRequest(action, Method.POST);
            request.AddHeader("X-CSRF-Token", CRSF_TOKEN);
            return commandClient.Execute(request).Content;

        }
        public string stopClimate() {

            RestRequest req = new RestRequest("/-/emanager/trigger-climatisation", Method.POST);
            req.AddJsonBody(new { triggerAction = false, electricClima = true });
            req.AddHeader("X-CSRF-Token", CRSF_TOKEN);
            return commandClient.Execute(req).Content;
        }
        public string startClimate()
        {
            RestRequest req = new RestRequest("/-/emanager/trigger-climatisation",Method.POST);
            req.AddJsonBody(new { triggerAction = true, electricClima = true });
            req.AddHeader("X-CSRF-Token", CRSF_TOKEN);
            return commandClient.Execute(req).Content;
            
        }

        private string extract_csrf(string htmlpage)
        {
            string startstring = "<meta name=\"_csrf\" content=\"";
            string endstring = "\"/>";
            return extract(htmlpage, startstring, endstring);
        }
        private string extract_redirect_url(string htmlpage)
        {
            string startstring = "<redirect url=\"";
            string endstring = "\"></redirect>";
            return extract(htmlpage, startstring, endstring);
        }
        private string extract_view_state(string htmlpage)
        {
            string startstring = "name=\"javax.faces.ViewState\" id=\"j_id1:javax.faces.ViewState:0\" value=\"";
            string endstring = "\"";
            return extract(htmlpage, startstring, endstring);
        }
        private string extract(string html, string startstring, string endstring)
        {
            int startplace = html.IndexOf(startstring);
            int startcut = startplace + startstring.Length;
            int endplace = html.IndexOf(endstring, startcut);
            return html.Substring(startcut, endplace - startcut);
        }
    }
}
