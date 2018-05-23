﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Odonto.DAO;
using Odonto.Models;
using Odonto.WebApp.Helpers.Auth;
using System;

namespace Odonto.WebApp.Controllers
{
    [TypeFilter(typeof(IsLoggedAttribute))]
    public class PatientsController : Controller
    {
        private PatientsDAO PatientsDAO;

        IConfiguration config;

        public PatientsController(IConfiguration _config)
        {
            config = _config;
            PatientsDAO = new PatientsDAO(config.GetSection("DB").GetSection("ConnectionString").Value);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Criar Novo";

            var Model = new Patient();

            return View(Model);
        }

        [HttpPost]
        public IActionResult Add(Patient Model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Section"] = "Pacientes";
                ViewData["Action"] = "Criar Novo";
                return View(Model);
            }

            Model.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            Model.CreatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("userId"));
            int id = PatientsDAO.Add(Model);

            return RedirectToAction("Record", new { id });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Editar";

            var Model = PatientsDAO.GetById(id);

            return View("Add", Model);
        }

        [HttpPost]
        public IActionResult Edit(Patient Model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Section"] = "Pacientes";
                ViewData["Action"] = "Editar";
                return View("Add", Model);
            }

            Model.ClinicID = Convert.ToInt32(HttpContext.Session.GetInt32("clinicId"));
            Model.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("userId"));
            PatientsDAO.Edit(Model);

            return RedirectToAction("Record", new { id = Model.ID});
        }
        [HttpGet]
        public IActionResult Record(int id)
        {
            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Prontuário Odontológico";

            var patient = PatientsDAO.GetById(id);

            return View(patient);
        }

        public IActionResult List()
        {
            ViewData["Section"] = "Pacientes";
            ViewData["Action"] = "Listar";

            var patientList = PatientsDAO.GetAll(Convert.ToInt32(HttpContext.Session.GetInt32("clinicId")));

            return View(patientList);
        }
    }
}