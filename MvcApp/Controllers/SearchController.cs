using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Newtonsoft.Json;
using Models;
using Newtonsoft.Json.Linq;
using MvcThrottle;
using Nest;
using Elasticsearch.Net;

namespace MvcApp.Controllers
{
    public class SearchController : BaseController
    {
        readonly SearchManager sManager = new SearchManager();

        // 初始搜索界面
        public ActionResult OriginSearch()
        {
            return View();
        }

        //搜索功能
        [HttpGet]
        [EnableThrottling(PerSecond = 3, PerMinute = 40, PerHour = 300, PerDay = 2000)]
        public ActionResult Search(string KeyWord)
        {
            string name;
            if (KeyWord == null)
            {
                return RedirectToAction("OriginSearch", "Search");
            }
            else
            {
                if (Request.Cookies["Login"] == null || Request.Cookies["Key"] == null)        //未登录时的情况
                {
                    name = "";
                }
                else
                {
                    HttpCookie cookie = Request.Cookies["Login"];
                    string tokenContent = cookie.Values["Token"];
                    string pubKey = Request.Cookies["Key"].Value;
                    if (VerToken(tokenContent, pubKey))
                    {
                        JObject username = readtoken(cookie.Values["Token"]);
                        name = username["UserName"].ToString();
                        ViewBag.username = name;
                        ViewBag.userid = username["UserId"].ToString();
                    }
                    else
                    {
                        name = "";
                    }
                }
                ViewBag.keyword = KeyWord;
                sManager.Search(KeyWord, name);
                return View();
            }
        }

        #region 自定义json返回类型
        public class JSONNetResult : ActionResult
        {
            private readonly JObject _data;
            public JSONNetResult(JObject data)
            {
                _data = data;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.ContentType = "application/json";
                response.Write(_data.ToString(Newtonsoft.Json.Formatting.None));
            }
        }
        #endregion

        #region 搜索功能

        #region 搜索测评
        [HttpGet]
        [EnableThrottling(PerSecond = 4, PerMinute = 500, PerHour = 3000, PerDay = 4000)]
        public ActionResult getEva(string keyWord, int? page, int? size)
        {
            page = page ?? 0;
            size = size ?? 20;
            if (size > 30)
                return Content("The size is limited");
            var setting = new ConnectionSettings(new Uri(@"http://127.0.0.1:9200")).DefaultIndex("evaluation");
            var lowClient = new ElasticLowLevelClient(setting);
            var search = lowClient.Search<BytesResponse>("evaluation", @"{
              ""from"": " + page.ToString() + @",
              ""size"": " + size.ToString() + @",
              ""query"": {
                ""bool"": {
                  ""should"": [
                    {
                      ""match"": {
                        ""content"": """ + keyWord + @"""
                      }
                    },
                    {
                      ""match"": {
                        ""aname"": """ + keyWord + @"""
                      }
                    },
                    {
                      ""match"": {
                        ""title"": """ + keyWord + @"""
                      }
                    },
                    {
                      ""match"": {
                        ""username"": """ + keyWord + @"""
                      }
                    }
                  ]
                }
              },
              ""highlight"": {
                  ""pre_tags"":""<span class='key' style='color:red;'>"",
                  ""post_tags"":""</span>"",
                  ""fields"":{
                    ""content"":{
                        ""fragment_size"": 150,
                        ""number_of_fragments"": 3
                    },
                    ""aname"":{
                        ""fragment_size"": 15,
                        ""number_of_fragments"": 3
                    },
                    ""title"":{
                        ""fragment_size"": 150,
                        ""number_of_fragments"": 3
                    },
                    ""username"":{
                        ""fragment_size"": 15,
                        ""number_of_fragments"": 3
                    }
                  }
              }
            }");
            var OriResult = search.Body;
            string thing = System.Text.Encoding.UTF8.GetString(OriResult);
            JObject OriList = (JObject)JsonConvert.DeserializeObject(thing);
            List<JObject> list = new List<JObject>();
            foreach (var item in OriList["hits"]["hits"])
            {
                JObject temp = new JObject();
                temp.Add("source", JToken.FromObject(item["_source"]));
                temp.Add("highlight", JToken.FromObject(item["highlight"]));
                list.Add(temp);
            }
            JObject result = new JObject();
            result.Add("count", OriList["hits"]["hits"].Count());
            result.Add("result", (JToken)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(list)));
            //Response.Write(JsonConvert.SerializeObject(result));
            return new JSONNetResult(result);
        }
        #endregion

        #region 搜索动漫
        [HttpGet]
        [EnableThrottling(PerSecond = 4, PerMinute = 500, PerHour = 3000, PerDay = 4000)]
        public ActionResult getAni(string keyWord, int? page, int? size)
        {
            page = page ?? 0;
            size = size ?? 20;
            if (size > 30)
                return Content("The size is limited");
            var setting = new ConnectionSettings(new Uri(@"http://127.0.0.1:9200")).DefaultIndex("evaluation");
            var lowClient = new ElasticLowLevelClient(setting);
            var search = lowClient.Search<BytesResponse>("animation", @"{
              ""from"": " + page.ToString() + @",
              ""size"": " + size.ToString() + @",
              ""query"": {
                ""bool"": {
                  ""should"": [
                    {
                      ""match"": {
                        ""aname"": """ + keyWord + @"""
                      }
                    },
                    {
                      ""match"": {
                        ""amainactor"": """ + keyWord + @"""
                      }
                    },
                    {
                      ""match"": {
                        ""alocation"": """ + keyWord + @"""
                      }
                    },
                    {
                      ""match"": {
                        ""anickname"": """ + keyWord + @"""
                      }
                    },
                    {
                      ""match"": {
                        ""awriter"": """ + keyWord + @"""
                      }
                    },
                    {
                      ""match"": {
                        ""adirector"": """ + keyWord + @"""
                      }
                    }
                  ]
                }
              },
              ""_source"": [""aname"",""aimagepic"",""animationid""]
            }");
            var OriResult = search.Body;
            string thing = System.Text.Encoding.UTF8.GetString(OriResult);
            JObject OriList = (JObject)JsonConvert.DeserializeObject(thing);
            List<JObject> list = new List<JObject>();
            foreach (var item in OriList["hits"]["hits"])
            {
                list.Add((JObject)item["_source"]);
            }
            JObject result = new JObject();
            result.Add("count", OriList["hits"]["hits"].Count());
            result.Add("result", (JToken)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(list)));
            //Response.Write(JsonConvert.SerializeObject(list));
            return new JSONNetResult(result);
        }
        #endregion

        #endregion

        #region 搜索建议
        [HttpGet]
        [EnableThrottling(PerSecond = 8)]
        public ActionResult getSuggest(string keyWord)
        {
            var setting = new ConnectionSettings(new Uri(@"http://127.0.0.1:9200")).DefaultIndex("evaluation");
            var lowClient = new ElasticLowLevelClient(setting);
            var suggestEva = lowClient.Search<BytesResponse>("evaluation", @"{
              ""suggest"": {
                ""name-suggest"": {
                  ""prefix"":""" + keyWord + @""",
                  ""completion"":{
                    ""field"":""suggest""
                    }
                }
              }
            }");
            var suggestAni = lowClient.Search<BytesResponse>("animation", @"{
              ""suggest"": {
                ""name-suggest"": {
                  ""prefix"":""" + keyWord + @""",
                  ""completion"":{
                    ""field"":""suggest""
                    }
                }
              }
            }");
            var resultEva = suggestEva.Body;
            var resultAni = suggestAni.Body;
            string thingEva = System.Text.Encoding.UTF8.GetString(resultEva);
            string thingAni = System.Text.Encoding.UTF8.GetString(resultAni);
            JObject OriEva = (JObject)JsonConvert.DeserializeObject(thingEva);
            JObject OriAni = (JObject)JsonConvert.DeserializeObject(thingAni);
            List<string> list = new List<string>();
            foreach (var item in OriEva["suggest"]["name-suggest"])
            {
                foreach (var item1 in item["options"])
                {
                    list.Add(item1["text"].ToString());
                }
            }
            foreach (var item in OriAni["suggest"]["name-suggest"])
            {
                foreach (var item1 in item["options"])
                {
                    list.Add(item1["text"].ToString());
                }
            }
            list = list.Distinct().ToList();
            JObject result = new JObject();
            result.Add("suggestion", (JToken)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(list)));
            //Response.Write(JsonConvert.SerializeObject(list));
            return new JSONNetResult(result);
        }
        #endregion

        //获取测评信息
        [HttpGet]
        public JsonResult GetEvaluation(int id, int type)
        {
            var theLongth = 300;
            if (type == 1)          //长串
            {
                string evaluation = sManager.GetEvaluationPreview(id);
                if (evaluation.Length < 300)
                {
                    theLongth = evaluation.Length;
                    evaluation = "";
                }
                else
                {
                    evaluation = evaluation.Substring(theLongth, evaluation.Length - 300);
                }
                return Json(evaluation, JsonRequestBehavior.AllowGet);
            }
            else                    //短串
            {
                string evaluation = sManager.GetEvaluationPreview(id);
                if (evaluation.Length < 300)
                {
                    theLongth = evaluation.Length;
                }
                evaluation = evaluation.Substring(0, theLongth);
                return Json(evaluation, JsonRequestBehavior.AllowGet);
            }
        }

        //搜索测评分布视图
        public ActionResult SearchUsers()
        {
            IEnumerable<tempSearchUsers> users = sManager.SearchUsers();
            return PartialView(users);
        }
    }
}