#!/bin/bash
# wait-for-rabbitmq.sh

set -e

host="rabbitmq"
shift
cmd=("$@")  # Store command and arguments as an array

# RabbitMQ credentials
RABBITMQ_USER="guest"
RABBITMQ_PASS="guest"

# Function to check RabbitMQ status
check_rabbitmq() {
  local status_code
  echo "Checking RabbitMQ status at http://${host}:15672/api/healthchecks/node"
  status_code=$(curl -u "${RABBITMQ_USER}:${RABBITMQ_PASS}" -s -o /dev/null -w "%{http_code}" "http://${host}:15672/api/healthchecks/node")
  
  if [ "$status_code" -eq 000 ]; then
    echo "Failed to connect to RabbitMQ. Retrying..."
    return 1
  fi
  
  echo "Received status code: $status_code"
  [ "$status_code" -eq 200 ]
}

# Maximum number of retries
max_retries=5
retry_count=0

# Wait until RabbitMQ is available
until check_rabbitmq; do
  if [ "$retry_count" -ge "$max_retries" ]; then
    >&2 echo "RabbitMQ is still unavailable after $((retry_count * 10)) seconds - exiting"
    exit 1
  fi

  >&2 echo "RabbitMQ is unavailable - sleeping"
  sleep 10
  retry_count=$((retry_count + 1))
done

>&2 echo "RabbitMQ is up - executing command"
exec "${cmd[@]}"
