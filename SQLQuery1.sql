use AuditSentinelDB;

--insert into Escaneos(NombreEscaneo, Estado, FechaEscaneo) values
--('EscaneoRedInterna_01', 'Completado', GETDATE()),
--('EscaneoWebExterna_02', 'En Proceso', GETDATE()),
--('EscaneoServidoresAD_03', 'Fallido', GETDATE()),
--('EscaneoBaseDatos_04', 'Completado', GETDATE()),
--('EscaneoFirewall_05', 'EnProceso', GETDATE()),
--('EscaneoCorreo_06', 'Completado', GETDATE()),
--('EscaneoBackup_07', 'Fallido', GETDATE());

select * from Escaneos;
select * from Plantillas;

update Escaneos set Estado = 'EnProgreso' where IdEscaneo=8;

delete from Escaneos where IdEscaneo > 10;

--insert into EscaneosServidores(IdServidor,IdEscaneo)
--values 
--(1, 1), (1, 2), (1, 3), (1, 4),(1, 5),
--(2, 6), (2,7),(2, 8), (2,9),(2, 10);

insert into EscaneosPlantillas(IdEscaneo, IdPlantilla)
values(5, 1), (6, 2), (7, 3),(8, 4), (9, 5), (10, 6);
