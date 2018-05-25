USE BSKproj2
GO

INSERT INTO Uzytkownicy VALUES(0,'admin','d033e22ae348aeb5660fc2140aec35850c4da997','Admin','Admninowski','admin@student.pg.edu.pl','false','');

INSERT INTO _Role VALUES(0,'Administrator');
INSERT INTO _Role VALUES(1,'Pacjent');
INSERT INTO _Role VALUES(2,'Recepcjonista');
INSERT INTO _Role VALUES(3,'Lekarz');

INSERT INTO Uzytkownicy_Role VALUES(0,0,0);
INSERT INTO Uzytkownicy_Role VALUES(1,0,1);
INSERT INTO Uzytkownicy_Role VALUES(2,0,2);
INSERT INTO Uzytkownicy_Role VALUES(3,0,3);

INSERT INTO Tabele VALUES(0,'Pacjenci');
INSERT INTO Tabele VALUES(1,'Karty_pacjentow');
INSERT INTO Tabele VALUES(2,'Wizyty');
INSERT INTO Tabele VALUES(3,'Lekarze');
INSERT INTO Tabele VALUES(4,'Diagnozy');
INSERT INTO Tabele VALUES(5,'Recepty');
INSERT INTO Tabele VALUES(6,'Leki');
INSERT INTO Tabele VALUES(7,'Lek_Recepta');
--Tylko dla administratora
INSERT INTO Tabele VALUES(8,'Uzytkownicy');
INSERT INTO Tabele VALUES(9,'_Role');
INSERT INTO Tabele VALUES(10,'Uzytkownicy_Role');
INSERT INTO Tabele VALUES(11,'Tabele');
INSERT INTO Tabele VALUES(12,'Role_Tabele');

----Administrator
INSERT INTO Role_Tabele VALUES(0,0,0,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(1,0,1,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(2,0,2,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(3,0,3,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(4,0,4,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(5,0,5,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(6,0,6,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(7,0,7,'true','true','true','true','false','')
--
INSERT INTO Role_Tabele VALUES(8,0,8,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(9,0,9,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(10,0,10,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(11,0,11,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(12,0,12,'true','true','true','true','false','')
----Pacjent
INSERT INTO Role_Tabele VALUES(13,1,0,'true','false','true','false','true',
	'SELECT Pacjenci.* FROM Pacjenci WHERE PESEL=<tuPESEL>')
INSERT INTO Role_Tabele VALUES(14,1,1,'true','false','false','false','true',
	'SELECT Karty_pacjentow.* FROM Pacjenci, Karty_pacjentow WHERE Karty_pacjentow.FK_Pacjent=Pacjenci.PESEL AND PESEL=<tuPESEL>')
INSERT INTO Role_Tabele VALUES(15,1,2,'true','false','false','false','true',
	'SELECT Wizyty.* FROM Pacjenci, Karty_pacjentow, Wizyty WHERE Wizyty.FK_Karta_pacjenta=Karty_pacjentow.nr_karty AND Karty_pacjentow.FK_Pacjent=Pacjenci.PESEL AND PESEL=<tuPESEL>')
INSERT INTO Role_Tabele VALUES(16,1,3,'true','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(17,1,4,'true','false','false','false','true',
	'SELECT Diagnozy.* FROM Pacjenci, Karty_pacjentow, Wizyty, Diagnozy WHERE Diagnozy.Id_diagnozy=Wizyty.FK_Diagnoza AND Wizyty.FK_Karta_pacjenta=Karty_pacjentow.nr_karty AND Karty_pacjentow.FK_Pacjent=Pacjenci.PESEL AND PESEL=<tuPESEL>')
INSERT INTO Role_Tabele VALUES(18,1,5,'true','false','false','false','true',
	'SELECT Recepty.* FROM Pacjenci, Karty_pacjentow, Wizyty, Recepty WHERE Recepty.FK_Wizyta=Wizyty.Id_wizyty AND Wizyty.FK_Karta_pacjenta=Karty_pacjentow.nr_karty AND Karty_pacjentow.FK_Pacjent=Pacjenci.PESEL AND PESEL=<tuPESEL>')
INSERT INTO Role_Tabele VALUES(19,1,6,'true','false','false','false','true',
	'SELECT Leki.* FROM Pacjenci, Karty_pacjentow, Wizyty, Recepty, Lek_Recepta, Leki WHERE Leki.Id_leku=Lek_Recepta.FK_Lek AND Lek_Recepta.FK_Recepta=Recepty.nr_recepty AND Recepty.FK_Wizyta=Wizyty.Id_wizyty AND Wizyty.FK_Karta_pacjenta=Karty_pacjentow.nr_karty AND Karty_pacjentow.FK_Pacjent=Pacjenci.PESEL AND PESEL=<tuPESEL>')
INSERT INTO Role_Tabele VALUES(20,1,7,'true','false','false','false','true',
	'SELECT Lek_Recepta.* FROM Pacjenci, Karty_pacjentow, Wizyty, Recepty, Lek_Recepta WHERE Lek_Recepta.FK_Recepta=Recepty.nr_recepty AND Recepty.FK_Wizyta=Wizyty.Id_wizyty AND Wizyty.FK_Karta_pacjenta=Karty_pacjentow.nr_karty AND Karty_pacjentow.FK_Pacjent=Pacjenci.PESEL AND PESEL=<tuPESEL>')
--
INSERT INTO Role_Tabele VALUES(21,1,8,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(22,1,9,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(23,1,10,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(24,1,11,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(25,1,12,'false','false','false','false','false','')
----Recepcjonista
INSERT INTO Role_Tabele VALUES(26,2,0,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(27,2,1,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(28,2,2,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(29,2,3,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(30,2,4,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(31,2,5,'true','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(32,2,6,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(33,2,7,'true','false','false','false','false','')
--
INSERT INTO Role_Tabele VALUES(34,2,8,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(35,2,9,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(36,2,10,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(37,2,11,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(38,2,12,'false','false','false','false','false','')
----Lekarz
INSERT INTO Role_Tabele VALUES(39,3,0,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(40,3,1,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(41,3,2,'true','true','true','true','true',
	'SELECT Wizyty.* FROM Lekarze, Wizyty WHERE Wizyty.FK_Lekarz=Lekarze.PESEL AND Lekarze.PESEL=<tuPESEL>')
INSERT INTO Role_Tabele VALUES(42,3,3,'true','true','true','true','true',
	'SELECT Lekarze.* FROM Lekarze WHERE Lekarze.PESEL=<tuPESEL>')
INSERT INTO Role_Tabele VALUES(43,3,4,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(44,3,5,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(45,3,6,'true','true','true','true','false','')
INSERT INTO Role_Tabele VALUES(46,3,7,'true','true','true','true','false','')
--
INSERT INTO Role_Tabele VALUES(47,3,8,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(48,3,9,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(49,3,10,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(50,3,11,'false','false','false','false','false','')
INSERT INTO Role_Tabele VALUES(51,3,12,'false','false','false','false','false','')
