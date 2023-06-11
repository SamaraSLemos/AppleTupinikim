using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TupinikimAppleCrud.Models;

namespace TupinikimAppleCrud.Controllers
{
    public class ProdutoController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "YHdCmf0t845VDewVgw4T2UBxA9OC79RV2sQaZ9Al", //database secret
            BasePath = "https://applestoretupinikim-default-rtdb.firebaseio.com"
        };
        IFirebaseClient client;


        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Produtos");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Produto>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Produto>(((JProperty)item).Value.ToString()));
                }
            }

            return View(list);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Produto produto)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var data = produto;
                PushResponse response = client.Push("Produtos/", data);
                data.Id = response.Result.name;
                SetResponse setResponse = client.Set("Produtos/" + data.Id, data);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError(string.Empty, "Adicionado com sucesso!");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Algo deu errado!");
                }


            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Produtos/" + id);
            Produto data = JsonConvert.DeserializeObject<Produto>(response.Body);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Produto produto)
        {
            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("Produtos/" + produto.Id, produto);
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Produtos/" + id);
            return RedirectToAction("Index");
        }
    }
}
