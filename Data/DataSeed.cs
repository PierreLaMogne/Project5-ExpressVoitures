using Microsoft.EntityFrameworkCore;
using Net_P5.Models;

namespace Net_P5.Data
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Vérifier si des marques existent déjà
            if (!context.Marques.Any())
            {
                var marques = new List<Marque>
                    {
                        new Marque { Nom = "Mazda" },
                        new Marque { Nom = "Jeep" },
                        new Marque { Nom = "Renault" },
                        new Marque { Nom = "Ford" },
                        new Marque { Nom = "Honda" },
                        new Marque { Nom = "Volkswagen" }
                    };
                context.Marques.AddRange(marques);
                context.SaveChanges();
            }

            // Vérifier si des modèles existent déjà
            if (!context.Modeles.Any())
            {
                var marques = context.Marques.ToList();

                var modeles = new List<Modele>
                    {
                        new Modele { Nom = "Miata", MarqueId = marques.Single(m => m.Nom=="Mazda").Id },
                        new Modele { Nom = "Liberty", MarqueId = marques.Single(m => m.Nom=="Jeep").Id },
                        new Modele { Nom = "Scénic", MarqueId = marques.Single(m => m.Nom=="Renault").Id },
                        new Modele { Nom = "Explorer", MarqueId = marques.Single(m => m.Nom=="Ford").Id },
                        new Modele { Nom = "Civic", MarqueId = marques.Single(m => m.Nom=="Honda").Id },
                        new Modele { Nom = "GTI", MarqueId = marques.Single(m => m.Nom=="Volkswagen").Id },
                        new Modele { Nom = "Edge", MarqueId = marques.Single(m => m.Nom=="Ford").Id }
                    };
                context.Modeles.AddRange(modeles);
                context.SaveChanges();
            }

            // Vérifier si des finitions existent déjà
            if (!context.Finitions.Any())
            {
                var modeles = context.Modeles.ToList();

                var finitions = new List<Finition>
                    {
                        new Finition { Nom = "LE", ModeleId = modeles.Single(mo => mo.Nom=="Miata").Id },
                        new Finition { Nom = "Sport", ModeleId = modeles.Single(mo => mo.Nom=="Liberty").Id },
                        new Finition { Nom = "TCe", ModeleId = modeles.Single(mo => mo.Nom=="Scénic").Id },
                        new Finition { Nom = "XLT", ModeleId = modeles.Single(mo => mo.Nom=="Explorer").Id },
                        new Finition { Nom = "LX", ModeleId = modeles.Single(mo => mo.Nom=="Civic").Id },
                        new Finition { Nom = "S", ModeleId = modeles.Single(mo => mo.Nom=="GTI").Id },
                        new Finition { Nom = "SEL", ModeleId = modeles.Single(mo => mo.Nom=="Edge").Id }
                    };
                context.Finitions.AddRange(finitions);
                context.SaveChanges();
            }

            // Vérifier si des voitures existent déjà
            if (!context.Voitures.Any())
            {
                var marques = context.Marques.ToList();
                var modeles = context.Modeles.ToList();
                var finitions = context.Finitions.ToList();

                var voitures = new List<Voiture>
                    {
                        new Voiture { CodeVIN = "VF1MAZDA000000001", Annee = 2019, DateAchat = new DateOnly(2022, 1, 7), PrixAchat = 1800m, FinitionId = finitions.Single(f => f.Nom=="LE").Id, EnVente = false },
                        new Voiture { CodeVIN = "VF1JEEP0000000002", Annee = 2007, DateAchat = new DateOnly(2022, 4, 2), PrixAchat = 4500m, FinitionId = finitions.Single(f => f.Nom=="Sport").Id, EnVente = false },
                        new Voiture { CodeVIN = "VF1RENAULT0000003", Annee = 2007, DateAchat = new DateOnly(2022, 4, 4), PrixAchat = 1800m, FinitionId = finitions.Single(f => f.Nom=="TCe").Id, EnVente = false },
                        new Voiture { CodeVIN = "VF1F0RD0000000004", Annee = 2017, DateAchat = new DateOnly(2022, 4, 5), PrixAchat = 24350m, FinitionId = finitions.Single(f => f.Nom=="XLT").Id, EnVente = false },
                        new Voiture { CodeVIN = "VF1H0NDA000000005", Annee = 2008, DateAchat = new DateOnly(2022, 4, 6), PrixAchat = 4000m, FinitionId = finitions.Single(f => f.Nom=="LX").Id, EnVente = false },
                        new Voiture { CodeVIN = "VF1V0LKSWAGEN0006", Annee = 2016, DateAchat = new DateOnly(2022, 4, 6), PrixAchat = 15250m, FinitionId = finitions.Single(f => f.Nom=="S").Id, EnVente = false },
                        new Voiture { CodeVIN = "VF1F0RD0000000007", Annee = 2013, DateAchat = new DateOnly(2022, 4, 7), PrixAchat = 10990m, FinitionId = finitions.Single(f => f.Nom=="SEL").Id, EnVente = false }
                    };
                context.Voitures.AddRange(voitures);
                context.SaveChanges();
            }

            // Vérifier si des réparations existent déjà
            if (!context.Reparations.Any())
            {

                var voitures = context.Voitures.ToList();

                var reparations = new List<Reparation>
                    {
                        new Reparation { Detail = "Restauration complète", VoitureId = voitures.Single(v => v.CodeVIN == "VF1MAZDA000000001").Id, Cout = 7600m, DateDisponibilite = new DateOnly(2022, 4, 7) },
                        new Reparation { Detail = "Roulements des roues avant", VoitureId = voitures.Single(v => v.CodeVIN == "VF1JEEP0000000002").Id, Cout = 350m, DateDisponibilite = new DateOnly(2022, 4, 7) },
                        new Reparation { Detail = "Radiateur, freins", VoitureId = voitures.Single(v => v.CodeVIN == "VF1RENAULT0000003").Id, Cout = 690m, DateDisponibilite = new DateOnly(2022, 4, 8) },
                        new Reparation { Detail = "Pneus, freins", VoitureId = voitures.Single(v => v.CodeVIN == "VF1F0RD0000000004").Id, Cout = 1100m, DateDisponibilite = new DateOnly(2022, 4, 9) },
                        new Reparation { Detail = "Climatisation, freins", VoitureId = voitures.Single(v => v.CodeVIN == "VF1H0NDA000000005").Id, Cout = 475m, DateDisponibilite = new DateOnly(2022, 4, 9) },
                        new Reparation { Detail = "Pneus", VoitureId = voitures.Single(v => v.CodeVIN == "VF1V0LKSWAGEN0006").Id, Cout = 440m, DateDisponibilite = new DateOnly(2022, 4, 10) },
                        new Reparation { Detail = "Pneus, freins, climatisation", VoitureId = voitures.Single(v => v.CodeVIN == "VF1F0RD0000000007").Id, Cout = 950m, DateDisponibilite = new DateOnly(2022, 4, 11) }
                    };
                context.Reparations.AddRange(reparations);
                context.SaveChanges();
            }

            // Vérifier si des ventes existent déjà
            if (!context.Ventes.Any())
            {
                var voitures = context.Voitures.ToList();
                var ventes = new List<Vente>
                    {
                        new Vente { VoitureId = voitures.Single(v => v.CodeVIN == "VF1MAZDA000000001").Id, DateVente = new DateOnly(2022, 4, 8)},
                        new Vente { VoitureId = voitures.Single(v => v.CodeVIN == "VF1JEEP0000000002").Id, DateVente = new DateOnly(2022, 4, 9)},
                        new Vente { VoitureId = voitures.Single(v => v.CodeVIN == "VF1H0NDA000000005").Id, DateVente = new DateOnly(2022, 4, 9)},
                        new Vente { VoitureId = voitures.Single(v => v.CodeVIN == "VF1V0LKSWAGEN0006").Id, DateVente = new DateOnly(2022, 4, 12)},
                        new Vente { VoitureId = voitures.Single(v => v.CodeVIN == "VF1F0RD0000000007").Id, DateVente = new DateOnly(2022, 4, 12)}
                    };
                context.Ventes.AddRange(ventes);
                context.SaveChanges();
            }
        }
    }
}