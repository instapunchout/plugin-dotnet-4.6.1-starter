using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace InstaPunchout.Controllers
{
    public class PunchoutController : Controller
    {

        private static readonly HttpClient client = new HttpClient();


        [HttpGet]
        public ActionResult Index()
        {
            var proxyRequest = new ProxyRequest();
            // store the query strings parameters into a dictionary in proxyRequest.query;
            foreach (string key in Request.QueryString.AllKeys)
            {
                proxyRequest.query.Add(key, Request.QueryString[key]);
            }

            // convert proxyRequest to json then to StringContent to be passed to httpClient.post
            var content = new StringContent(JsonSerializer.Serialize(proxyRequest), Encoding.UTF8, "application/json");

            // call instapunchout api with the proxyRequest as a body (json)
            var response = client.PostAsync("https://punchout.cloud/proxy", content).Result.Content.ReadAsStringAsync().Result;

            try
            {
                // parse instapunchout response into ProxyResponse
                ProxyResponse data =
                    JsonSerializer.Deserialize<ProxyResponse>(response);

                // the action is for extending future functionality, for now it is only login
                if (data.action == "login")
                {
                    // we try to retreive customer by email
                    var customer = GetCustomerByEmail(data.email);
                    if (customer == null)
                    {
                        // if it doesn't exist we create new one with the data received in the ProxyResponse
                        customer = CreateCustomer(data);
                    }
                    // after we have a customer object ready, we set it as logged in, this usually involves settings cookies and starting a session
                    SetCustomerAsLoggedIn(customer);

                    // after the customer is logged in, we store the punchout_id on the customer session, for later used when sending the cart to instapunchout
                    // and to inject the script on every page
                    SetPunchoutId(customer, data.punchout_id);

                    // redirect to home page or any other desired page, usually the catalog page so the user can start shopping
                    return Redirect("/");
                }
                else
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

            } catch (Exception e) {
                return Json(response + e.Message , JsonRequestBehavior.AllowGet);

            }
        }


        // executed by the javascript injected on the frontend
        // triggered when the user clicks on the "Punchout" (Take Cart Back) button
        [HttpGet]
        public ActionResult Cart()
        {
            // check if we are in a punchout session and get the punchout id
            var punchoutId = GetPunchoutId();
            if (punchoutId == null)
            {
                // if not in punchout session we should return the error message
                var cartResponse = new CartResponse();
                cartResponse.message = "This api is only available in a punchout session";
                return Json(cartResponse, JsonRequestBehavior.AllowGet);
            }
            // since we're in punchout session, the first thing is to get the cart object of the current user
            var cart = GetLoggedInCustomerCart();
            // usually cart objects in .NET are not serializable directly to json so we use this function to
            // convert them to json format, preferably keeping the same structure
            var cartJson = ConvertCartToJson(cart);

            // we construct the request body that we will send to instapunchout, this mainly include the cartJson
            // and also any query parameters received from frontend.
            var cartRequest = new CartRequest();
            cartRequest.custom = new Dictionary<string, string>();
            cartRequest.cart = new CartJsonWrapper();
            cartRequest.cart.Sanacommerce = cartJson;

            // we add query params to the .custom field
            foreach (string key in Request.QueryString.AllKeys)
            {
                cartRequest.custom.Add(key, Request.QueryString[key]);
            }

            // convert cart request to json and then to StringContent to be used with httpClient
            var content = new StringContent(JsonSerializer.Serialize(cartRequest), Encoding.UTF8, "application/json");

            // send cart request body to instapunchout
            var response = client.PostAsync("https://punchout.cloud/cart/" + punchoutId, content).Result.Content.ReadAsStringAsync().Result;

            try
            {
                // if the response format is correct (CartResponse) we pass it directly as response, the javascript on the frontend will use it
                // to redirect the user back to the procurement system
                CartResponse data = JsonSerializer.Deserialize<CartResponse>(response);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                // if we error out tryign to get the CartResponse, then we return back only the message field, which the javascript on frontend
                // will use as error message
                var cartResponse = new CartResponse();
                cartResponse.message = e.Message;
                return Json(cartResponse, JsonRequestBehavior.AllowGet);
            }
        }


        // notice the added: <script src="/punchout/script"></script> on the _Layout.cshtml
        // this script needs to be added on every page
        // if in punchout session it will load a javascript from instapunchout, if not it will be empty.
        [HttpGet]
        public ActionResult Script()
        {
            // check if we are in a punchout session and get the punchout id
            var punchoutId = GetPunchoutId();
            if (punchoutId == null) {
                // since we're not in a punchout session we will return an empty file
                return JavaScript("");
            }
            // since we're in a punchout session we will load the script from instapunchout
            // we need to pass the punchoutId as id query string
            var response = client.GetAsync("https://punchout.cloud/punchout.js?id=" + punchoutId).Result.Content.ReadAsStringAsync().Result;

            // return the javascript file content
            return JavaScript(response);
        }

        private Customer GetCustomerByEmail(string email)
        {
            // TODO: find customer by email
            if (email == "exists@punchout.cloud")
            {
                return new Customer();
            } else
            {
                return null;
            }
        }

        private Customer CreateCustomer(ProxyResponse data)
        {
            // TODO: create customer
            return new Customer();

        }

        private void SetCustomerAsLoggedIn(Customer customer)
        {
            // TODO: set the customer as logged in (set cookie/ session ...)
        }

        private void SetPunchoutId(Customer customer, string punchoutId)
        {
            // TODO: save the punchout id in the current logged in user session
        }

        private string GetPunchoutId()
        {
            // TODO: retreive the punchout id from the current logged in user if it exists
            return null;
        }

        private Cart GetLoggedInCustomerCart()
        {
            // TODO: pull the cart object of the currently logged in user
            return new Cart();
        }


        private CartJson ConvertCartToJson(Cart cart)
        {
            // TODO: optional conversion if the native cart can't be serialized to json directly
            return new CartJson();
        }


    }

    public class ProxyRequest
    {
        public Dictionary<String, String> headers { get; set; }
        public Dictionary<String, String> server { get; set; }
        public Dictionary<string, string> query { get; set; }
        public string body { get; set; }

        public ProxyRequest()
        {
            headers = new Dictionary<string, string>();
            server = new Dictionary<string, string>();
            query = new Dictionary<string, string>();
            body = "";
        }
    }

    public class ProxyResponse
    {
        public string action { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string group_id { get; set; }

        public string punchout_id { get; set; }
    }

    public class CartJsonWrapper
    {
        public CartJson Sanacommerce { get; set; }
    }

    public class CartRequest
    {
        public CartJsonWrapper cart { get; set; }
        public Dictionary<string, string> custom { get; set; }
    }

    public class CartResponse
    {
        public string url { get; set; }
        public Dictionary<string, string> data { get;set;}
        public string message { get; set; }
    }

    public class CartJson
    {
        // TODO: this is an optional class only used if the native Cart object of the ecommerce
        // can't be converted to json. in that case CartJson should mimick the structure of the native Cart object
        // as much as possible
    }
}