//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FromSoftwareStorage.EntityModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Folder
    {
        public Folder()
        {
            this.Games = new HashSet<Game>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public bool ReadOnly { get; set; }
    
        public virtual ICollection<Game> Games { get; set; }
    }
}