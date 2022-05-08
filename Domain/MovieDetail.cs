﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neverland.Domain
{
    public  class MovieDetail
    {
        [Key]
        public int Id { get; set; }

        public string? Description { get; set; }

        //[ForeignKey]
        public int MovieId { get; set; }

        public Movie Movie { get; set; }
    }
}
