USE BSKproj2
GO

CREATE TABLE Lekarze(
	PESEL				varchar(11)			PRIMARY KEY,
	imie				varchar(20),
	nazwisko			varchar(20),
	nr_gabinetu			varchar(10),
	telefon				varchar(12),
	email				varchar(50),
);

CREATE TABLE Pacjenci(
	PESEL				varchar(11)			PRIMARY KEY,
	imie				varchar(20),
	nazwisko			varchar(20),
	data_urodzenia		varchar(10),
	nr_telefonu			varchar(12),
	email				varchar(50),
	kod_pocztowy		varchar(6),
);

CREATE TABLE Karty_pacjentow(
	nr_karty			int					PRIMARY KEY,
	FK_Pacjent			varchar(11)			REFERENCES Pacjenci(PESEL) ON DELETE SET NULL ON UPDATE CASCADE,
	szczegoly_medyczne	varchar(140),
);

CREATE TABLE Diagnozy(
	Id_diagnozy			int					PRIMARY KEY,
	choroba_dolegliwosc	varchar(140),
);

CREATE TABLE Wizyty(
	Id_wizyty			int					PRIMARY KEY,
	FK_Lekarz			varchar(11)			REFERENCES Lekarze(PESEL) ON DELETE SET NULL ON UPDATE CASCADE,
	FK_Karta_pacjenta	int					REFERENCES Karty_pacjentow(nr_karty) ON DELETE SET NULL ON UPDATE CASCADE,
	FK_Diagnoza			int					REFERENCES Diagnozy(Id_diagnozy) ON DELETE SET NULL ON UPDATE CASCADE,
	data_wizyty			varchar(10),
	godzina_wizyty		varchar(8),
);

CREATE TABLE Leki(
	Id_leku				int					PRIMARY KEY,
	nazwa				varchar(50),
);

CREATE TABLE Recepty(
	nr_recepty			int					PRIMARY KEY,
	FK_Wizyta			int					REFERENCES Wizyty(Id_wizyty) ON DELETE SET NULL ON UPDATE CASCADE,
	data_wystawienia	varchar(10),
	data_waznosci		varchar(10),
);


CREATE TABLE Lek_Recepta(
	Id_lek_recepta		int					PRIMARY KEY,
	FK_Lek				int					REFERENCES Leki(Id_leku) ON DELETE SET NULL ON UPDATE CASCADE,
	FK_Recepta			int					REFERENCES Recepty(nr_recepty) ON DELETE SET NULL ON UPDATE CASCADE,
	dawka				varchar(140),
);

----------------

CREATE TABLE Uzytkownicy(
	Id_uzytkownika		int					PRIMARY KEY,
	_login				varchar(20)			UNIQUE,
	haslo				varchar(40),
	imie				varchar(20),
	nazwisko			varchar(20),
	email				varchar(50),
	czy_zalogowany		varchar(5),
	PESEL				varchar(11),
);

CREATE TABLE _Role(
	Id_roli				int					PRIMARY KEY,
	nazwa				varchar(20),
);

CREATE TABLE Uzytkownicy_Role(
	Id_Uzytkownicy_Role	int					PRIMARY KEY,
	FK_Uzytkownik		int					REFERENCES Uzytkownicy(Id_uzytkownika) ON DELETE SET NULL ON UPDATE CASCADE,
	FK_Rola				int					REFERENCES _Role(Id_roli) ON DELETE SET NULL ON UPDATE CASCADE,
);

CREATE TABLE Tabele(
	Id_tabeli			int					PRIMARY KEY,
	nazwa				varchar(20),
);

CREATE TABLE Role_Tabele(
	Id_Role_Tabele		int					PRIMARY KEY,
	FK_Rola				int					REFERENCES _Role(Id_roli) ON DELETE SET NULL ON UPDATE CASCADE,
	FK_Tabela			int					REFERENCES Tabele(Id_tabeli) ON DELETE SET NULL ON UPDATE CASCADE,
	selects				varchar(5),
	inserts				varchar(5),
	updates				varchar(5),
	deletes				varchar(5),
	tylkoWlasny			varchar(5),		
	tylkoWlasny_zapytanie varchar(512),
);