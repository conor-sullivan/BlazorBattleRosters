﻿using System.ComponentModel.DataAnnotations;

namespace BlazorBattleRosters.Data.Models
{
    public class RosterModel
    {
        public int Id { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
        public IEnumerable<UnitModel>? Units { get; set; }
    }
}