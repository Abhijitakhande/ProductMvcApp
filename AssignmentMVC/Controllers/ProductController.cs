using AssignmentMVC.Models;
using AssignmentMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentMVC.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ProductController : Controller
    {
        private readonly IProductRepository _repo;

        public ProductController(IProductRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            return Json(_repo.GetAll());
        }

        [HttpPost]
        public JsonResult Save(Product product)
        {
            try
            {
                if (product.Id == 0)
                    _repo.Add(product);
                else
                    _repo.Update(product);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            _repo.Delete(id);
            return Json(new { success = true });
        }
    }
}
