﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Com.DanLiris.Service.Purchasing.Lib.Facades.Expedition;
using Com.DanLiris.Service.Purchasing.Lib.Services;
using Com.DanLiris.Service.Purchasing.WebApi.Helpers;
using Com.DanLiris.Service.Purchasing.Lib.Models.Expedition;

namespace Com.DanLiris.Service.Purchasing.WebApi.Controllers.v1.Expedition
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/expedition/purchasing-document-expeditions")]
    [Authorize]
    public class PurchasingDocumentExpeditionController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly PurchasingDocumentExpeditionFacade purchasingDocumentExpeditionFacade;
        private readonly IdentityService identityService;

        public PurchasingDocumentExpeditionController(PurchasingDocumentExpeditionFacade purchasingDocumentExpeditionFacade, IdentityService identityService)
        {
            this.purchasingDocumentExpeditionFacade = purchasingDocumentExpeditionFacade;
            this.identityService = identityService;
        }

        [HttpGet]
        public ActionResult Get(int page = 1, int size = 25, string order = "{}", string keyword = null, string filter = "{}")
        {
            return new BaseGet<PurchasingDocumentExpeditionFacade>(purchasingDocumentExpeditionFacade)
                .Get(page, size, order, keyword, filter);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            this.identityService.Username = User.Claims.Single(p => p.Type.Equals("username")).Value;
            this.identityService.Token = Request.Headers["Authorization"].First().Replace("Bearer ", "");

            return await new BaseDelete<PurchasingDocumentExpeditionFacade>(purchasingDocumentExpeditionFacade, ApiVersion)
                .Delete(id);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model =  await purchasingDocumentExpeditionFacade.ReadModelById(Id);

            if (model == null)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.NOT_FOUND_STATUS_CODE, General.NOT_FOUND_MESSAGE)
                    .Fail();
                return NotFound(Result);
            }

            try
            {
                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = model,
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }

    }
}