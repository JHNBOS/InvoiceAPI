﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using InvoiceAPI.Components.Entities;
using InvoiceAPI.Components.Services;
using InvoiceAPI.Components.Services.Interfaces;
using InvoiceAPI.Controllers.ViewModels;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ShareListAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/users")]
    public class DebtorsController : Controller
    {
        private IDebtorRepository _repo;
        private IDebtorHasAddressRepository _debtorHasAddressRepo;
        private IAddressRepository _addressRepository;

        public DebtorsController()
        {
            this._repo = new DebtorRepository();
            this._debtorHasAddressRepo = new DebtorHasAddressRepository();
            this._addressRepository = new AddressRepository();
        }

        /// <summary>
        /// Gets a list with all debtors.
        /// </summary>
        [HttpGet("getDebtors")]
        [ProducesResponseType(typeof(IEnumerable<DebtorViewModel>), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetUsers()
        {
            //Get data
            var data = await _repo.GetDebtors();
            if (data == null)
            {
                return StatusCode(500, "Debtors could not be found.");
            }

            //Convert to viewmodel
            var result = data.Select(async s =>
            {
                var addressViewModel = new AddressViewModel();
                var address = await this.GetAddress(s.Id);
                addressViewModel.SetProperties(address);

                new DebtorViewModel
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    CompanyName = s.CompanyName,
                    BankAccount = s.BankAccount,
                    Phone = s.Phone,
                    Address = addressViewModel
                };
            });

            return Ok(result);
        }

        /// <summary>
        /// Gets a debtor by email.
        /// </summary>
        /// <param name="email">Email of debtor</param>
        [HttpGet("getByEmail")]
        [ProducesResponseType(typeof(UserViewModel), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetByEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return StatusCode(400, "Invalid parameter(s).");
            }

            // Check if email address is valid
            if (!Regex.Match(email, @"^([\w\.\-]+)@((?!\.|\-)[\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                return StatusCode(400, "This e-mail address is not valid.");
            }

            //Get debtor
            var data = await _repo.GetDebtorByEmail(email);
            if (data == null)
            {
                return StatusCode(500, "Debtor could not be found.");
            }

            //Convert to view model
            var result = new DebtorViewModel();
            result.SetProperties(data);

            return Ok(result);
        }

        /// <summary>
        /// Gets a debtor by id.
        /// </summary>
        /// <param name="email">Id of debtor</param>
        [HttpGet("getById")]
        [ProducesResponseType(typeof(UserViewModel), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetById(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return StatusCode(400, "Invalid parameter(s).");
            }

            //Get user
            var data = await _repo.GetDebtorByID(id);
            if (data == null)
            {
                return StatusCode(500, "Debtor could not be found.");
            }

            //Convert to view model
            var result = new DebtorViewModel();
            result.SetProperties(data);

            return Ok(result);
        }

        /// <summary>
        /// Creates a debtor.
        /// </summary>
        /// <param name="model">Debtor object</param>
        [HttpPost("create")]
        [ProducesResponseType(typeof(DebtorViewModel), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> Create([FromBody]DebtorViewModel model)
        {
            if (model == null)
            {
                return StatusCode(400, "Invalid parameter(s).");
            }

            Debtor debtor = new Debtor
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                BankAccount = model.BankAccount,
                Phone = model.Phone,
                CompanyName = model.CompanyName
            };

            //Insert debtor
            var data = await _repo.Insert(debtor);
            if (data == null)
            {
                return StatusCode(500, "A problem occured while saving the record. Please try again!");
            }

            var result = new DebtorViewModel();
            result.SetProperties(data);

            return Ok(result);
        }

        /// <summary>
        /// Updates a debtor.
        /// </summary>
        /// <param name="model">Debtor object</param>
        [HttpPut("update")]
        [ProducesResponseType(typeof(DebtorViewModel), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> Update([FromBody]DebtorViewModel model)
        {
            if (model == null)
            {
                return StatusCode(400, "Invalid parameter(s).");
            }

            Debtor debtor = new Debtor
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                BankAccount = model.BankAccount,
                Phone = model.Phone,
                CompanyName = model.CompanyName
            };

            //Update debtor
            var data = await _repo.Update(debtor);
            if (data == null)
            {
                return StatusCode(500, "A problem occured while updating the record. Please try again!");
            }

            var result = new DebtorViewModel();
            result.SetProperties(data);

            return Ok(result);
        }

        /// <summary>
        /// Deletes a debtor.
        /// </summary>
        /// <param name="email">Id of debtor</param>
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return StatusCode(400, "Invalid parameter(s).");
            }

            //Remove debtor
            var succeeded = await _repo.Delete(id);
            if (!succeeded)
            {
                return StatusCode(500, "A problem occured while removing the record. Please try again!");
            }

            return Ok("Success");
        }

        #region Private Methods

        private async Task<Address> GetAddress(string debtorId)
        {
            var debtorHasAddress = await this._debtorHasAddressRepo.GetAddressByDebtorId(debtorId);
            var address = await this._addressRepository.GetAddressByPostalAndNumber(debtorHasAddress.Number, debtorHasAddress.PostalCode);

            return address;
        }

        #endregion
    }
}
