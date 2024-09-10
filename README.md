Projeto apenas para fins de Profile(Perfil).

O objetivo aqui é demonstrar conhecimento nas ferramentas que estão sendo utilizadas e listadas abaixo:
C#, ASP.NET Core, Web Api Rest, XML (Dom), Mongo DB, SQL Server e RabbitMQ
Padrão: DDD e CQRS(Ainda não implementado)

Como funciona:
Após Upload de uma NFE o sistema envia uma mensagem para fila do Rabbit para que o documento possa ser processado,
Apos resgate da mensagem na fila, há uma Azure function que está plugada ao Rabbit e obtem o documento que deve ser processado.
A function por sua vez pega o documento e através de XML-Dom obtem os produtos do nó DET separando os itens Base de Calculo (VBC) e (PICMS)
gravando no MONGO DB e acionando o pipeline Agregate que através de uma formula executa o calculo criando um novo campo no icmsDivided com resultado
