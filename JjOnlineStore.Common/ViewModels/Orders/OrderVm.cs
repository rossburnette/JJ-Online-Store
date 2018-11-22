﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JjOnlineStore.Common.Enumeration;
using JjOnlineStore.Common.ViewModels.OrderItems;

namespace JjOnlineStore.Common.ViewModels.Orders
{
    public class OrderVm
    {
        public long Id { get; set; }

        public DateTime CreatedOn { get; set; }

        //Delivery Methods
        [Required(ErrorMessage = "Please enter your first name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter a country name.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please enter a city name.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter a state name.")]
        public string State { get; set; }

        public string Zip { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool Shipped { get; set; }

        //Payment Methods
        [Required(ErrorMessage = "Please enter a Cardholder Name.")]
        public string CardholderName { get; set; }

        [Required(ErrorMessage = "Please enter a Card Number.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Please enter a Expire Date.")]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }

        [Required(ErrorMessage = "Please enter a CW.")]
        public string Cvv { get; set; }

        public TransportationType TransportationType { get; set; }

        public IEnumerable<OrderItemVm> OrderedItems { get; set; }

        public long? InvoiceId { get; set; }

        public string UserId { get; set; }


        public decimal GrandTotal() =>
            OrderedItems.Sum(oi => oi.Product.Price * oi.Quantity);

        public OrderVm EncryptSensitiveData(OrderVm order)
        {
            order.CardholderName = 
        }
    }
}