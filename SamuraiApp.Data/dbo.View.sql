CREATE VIEW SamuraiBattleStats AS
SELECT SamuraiBattle.SamuraiId, Samurais.Name, COUNT(SamuraiBattle.BattleId) AS NumberOfBattles
FROM SamuraiBattle INNER JOIN Samurais ON SamuraiBattle.SamuraiId = Samurais.Id
GROUP BY Samurais.Name, SamuraiBattle.SamuraiId
