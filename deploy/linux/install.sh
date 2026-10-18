#!/usr/bin/env bash
set -euo pipefail

if [[ "${EUID}" -ne 0 ]]; then
  echo "Run this installer with sudo or as root." >&2
  exit 1
fi

if [[ "$#" -ne 1 || ! -d "$1" ]]; then
  echo "Usage: sudo bash install.sh <publish-directory>" >&2
  exit 1
fi

publish_dir="$(cd "$1" && pwd)"
if [[ ! -f "${publish_dir}/ERPBooking" ]]; then
  echo "No ERPBooking executable found in ${publish_dir}." >&2
  exit 1
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
service_file="${script_dir}/erpbooking.service"
if [[ ! -f "${service_file}" ]]; then
  echo "Service unit not found: ${service_file}" >&2
  exit 1
fi

if ! getent group erpbooking >/dev/null; then
  groupadd --system erpbooking
fi

if ! id erpbooking >/dev/null 2>&1; then
  useradd --system --gid erpbooking --no-create-home --home-dir /nonexistent --shell /usr/sbin/nologin erpbooking
fi

if systemctl is-active --quiet erpbooking.service; then
  systemctl stop erpbooking.service
fi

install -d --owner=root --group=root --mode=0755 /opt/erpbooking
cp -a "${publish_dir}/." /opt/erpbooking/
chown -R root:root /opt/erpbooking
chmod -R a+rX /opt/erpbooking
chmod 0755 /opt/erpbooking/ERPBooking

install -D --owner=root --group=root --mode=0644 "${service_file}" /etc/systemd/system/erpbooking.service
systemctl daemon-reload
systemctl enable erpbooking.service
systemctl restart erpbooking.service
systemctl --no-pager --full status erpbooking.service

echo "ERPBooking is listening on http://127.0.0.1:5080."
