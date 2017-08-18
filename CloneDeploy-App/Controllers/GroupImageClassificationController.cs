﻿using System.Collections.Generic;
using System.Web.Http;
using CloneDeploy_App.Controllers.Authorization;
using CloneDeploy_Entities;
using CloneDeploy_Entities.DTOs;
using CloneDeploy_Services;
using CloneDeploy_Common;

namespace CloneDeploy_App.Controllers
{
    public class GroupImageClassificationController : ApiController
    {
        private readonly GroupImageClassificationServices _groupImageClassificationServices;

        public GroupImageClassificationController()
        {
            _groupImageClassificationServices = new GroupImageClassificationServices();
        }

        [CustomAuth(Permission = AuthorizationStrings.UpdateGroup)]
        public ActionResultDTO Post(List<GroupImageClassificationEntity> listOfClassifications)
        {
            return _groupImageClassificationServices.AddClassifications(listOfClassifications);
        }

        [Authorize]
        [HttpPost]
        public IEnumerable<ImageWithDate> FilterClassifications(FilterGroupClassificationDTO filterDto)
        {
            return _groupImageClassificationServices.FilterClassifications(filterDto.GroupId, filterDto.ListImages);
        }
    }
}