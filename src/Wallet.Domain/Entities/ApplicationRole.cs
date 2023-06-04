﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Domain.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public string Description { get; set; }
}
