FROM rabbitmq:3-management

ADD ./rabbitmq.config /etc/rabbitmq/
ADD ./definitions.json /etc/rabbitmq/
HEALTHCHECK --interval=20s --timeout=5s --start-period=30s \
CMD rabbitmqctl status || exit 1