﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class Board
    {

        public int Id { get; set; }
        public List<BoardProject> Values { get; set; }

    }
}
