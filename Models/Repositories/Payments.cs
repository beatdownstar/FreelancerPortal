﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

using ProsjektoppgaveITE1811Gruppe7.Models.Entities;

namespace ProsjektoppgaveITE1811Gruppe7.Controllers
{
    public class Payments
    {
        static HttpClient client = new HttpClient();
        private readonly string ApiKey = "e1fc-defa-c0ae-da34";
        private readonly string Pin = "311080Bel";
        private readonly string Uri = "https://block.io/api/v2/";

        public Payments() { }

        private APIResponse apiCall(string Method, NameValueCollection Parameters)
        {
            WebClient Client = new WebClient();
            Uri url = new Uri(Uri + Method + "/?api_key=" + this.ApiKey);

            string JsonString = string.Empty;
            APIResponse Response = new APIResponse();
            // Parameters.Add("api_key", this.ApiKey);

            try
            {
                JsonString = Client.DownloadString(url + "&" + Parameters.ToString());
            }
            catch (WebException ex)
            {
                JsonString = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            }
            Response = JsonConvert.DeserializeObject<APIResponse>(JsonString);

            if (Response.Status == "success")
            {
                if (Response.Data.ContainsKey("user_id"))
                {
                    Response.Data["user_id"] = Convert.ToInt32(Response.Data["user_id"]);
                }
            }
            else
            {
                throw new Exception("Block.io API Error: " + Response.Data["error_message"]);
            }
            return Response;
        }

        private string CombineString(string DestinationString, string Item)
        {
            if (DestinationString == string.Empty)
            {
                DestinationString = Item;
            }
            else
            {
                DestinationString = DestinationString + "," + Item;
            }
            return DestinationString;
        }

        // <summary>
        /// Returns all the addresses, their labels, and user ids on your account. must specify addresses or labels.
        /// </summary>
        /// <returns>APIResponse</returns>
        public APIResponse getAddressBalance(string Type, List<string> Values)
        {
            NameValueCollection Params = System.Web.HttpUtility.ParseQueryString(string.Empty);
            string CombinedValues = string.Empty;
            if (Type != null)
            {
                foreach (string Item in Values)
                {
                    CombinedValues = CombineString(CombinedValues, Item);
                }
                Params.Add(Type, CombinedValues);
            }
            else
            {
                throw new Exception("Block.io API Error: you must enter a Type");
            }

            return apiCall("get_address_balance", Params);
        }

        /// <summary>
        /// Withdraws AMOUNT coins from upto 100 addresses at a time, and deposits it to up to 100 destination addresses.
        /// </summary>
        /// <returns></returns>
        public APIResponse withdrawFromAddresses(List<string> FromAddresses, List<string> ToAddresses, List<string> Amounts)
        {
            
            NameValueCollection Params = System.Web.HttpUtility.ParseQueryString(string.Empty);
            string CombinedFromAddresses = string.Empty;
            string CombinedToAddresses = string.Empty;
            string CombinedAmounts = string.Empty;
            foreach (string Item in FromAddresses)
            {
                CombinedFromAddresses = CombineString(CombinedFromAddresses, Item);
            }
            foreach (string Item in ToAddresses)
            {
                CombinedToAddresses = CombineString(CombinedToAddresses, Item);
            }
            foreach (string Item in Amounts)
            {
                CombinedAmounts = CombineString(CombinedAmounts, Item);
            }
            Params.Add("from_addresses", CombinedFromAddresses);
            Params.Add("to_addresses", CombinedToAddresses);
            Params.Add("amounts", CombinedAmounts);
            Params.Add("pin", Pin);
            return apiCall("withdraw_from_addresses", Params);
        }

        /// <summary>
        /// Returns a newly generated address, and its unique(!) label generated by Block.io. You can optionally specify a custom label.
        /// </summary>
        /// <param name="getNewAddress"></param>
        /// <returns>APIResponse</returns>
        public APIResponse getNewAddress(string label = null)
        {
            NameValueCollection Params = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (label != null)
            {
                Params.Add("label", label);
            }
            return apiCall("get_new_address", Params);
        }

        /// <summary>
        /// Returns various data for the last 100 transactions spent or received. You can optionally specify a before_tx parameter to get earlier transactions.
        /// </summary>
        /// <returns></returns>
        public APIResponse getTransactions(string Type, List<string> Addresses)
        {
            NameValueCollection Params = System.Web.HttpUtility.ParseQueryString(string.Empty);
            string CombinedAddresses = string.Empty;
            foreach (string Item in Addresses)
            {
                CombinedAddresses = CombineString(CombinedAddresses, Item);
            }
            Params.Add("type", Type);
            Params.Add("addresses", CombinedAddresses);

            return apiCall("get_transactions", Params);
        }
    }
}