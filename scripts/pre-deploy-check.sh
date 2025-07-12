#!/bin/bash

# 部署前檢查腳本
# 此腳本檢查伺服器環境是否已準備好進行部署

set -e

echo "?? 開始部署前環境檢查..."

# 檢查是否為 root 使用者
if [ "$EUID" -eq 0 ]; then
  echo "? 請不要使用 root 使用者執行此腳本"
  exit 1
fi

# 檢查 Docker 是否已安裝
if ! command -v docker &> /dev/null; then
    echo "? Docker 未安裝，請先安裝 Docker"
    echo "?? 安裝指令: curl -fsSL https://get.docker.com -o get-docker.sh && sudo sh get-docker.sh"
    exit 1
else
    echo "? Docker 已安裝: $(docker --version)"
fi

# 檢查 Docker Compose 是否已安裝
if ! command -v docker-compose &> /dev/null; then
    echo "? Docker Compose 未安裝，請先安裝 Docker Compose"
    echo "?? 安裝指令: sudo curl -L \"https://github.com/docker/compose/releases/latest/download/docker-compose-\$(uname -s)-\$(uname -m)\" -o /usr/local/bin/docker-compose && sudo chmod +x /usr/local/bin/docker-compose"
    exit 1
else
    echo "? Docker Compose 已安裝: $(docker-compose --version)"
fi

# 檢查 Docker 服務是否運行
if ! systemctl is-active --quiet docker; then
    echo "? Docker 服務未運行，請啟動 Docker 服務"
    echo "?? 啟動指令: sudo systemctl start docker"
    exit 1
else
    echo "? Docker 服務正在運行"
fi

# 檢查使用者是否在 docker 群組中
if ! groups | grep -q docker; then
    echo "? 當前使用者不在 docker 群組中"
    echo "?? 加入群組指令: sudo usermod -aG docker \$USER"
    echo "?? 之後請重新登入或執行: newgrp docker"
    exit 1
else
    echo "? 使用者已在 docker 群組中"
fi

# 檢查必要的目錄
APP_DIR="/home/$(whoami)/videosrtsearch"
if [ ! -d "$APP_DIR" ]; then
    echo "?? 建立應用程式目錄: $APP_DIR"
    mkdir -p "$APP_DIR"
    mkdir -p "$APP_DIR/logs"
    mkdir -p "$APP_DIR/wwwroot/uploads"
else
    echo "? 應用程式目錄已存在: $APP_DIR"
fi

# 檢查生產環境設定檔
CONFIG_FILE="$APP_DIR/appsettings.Production.json"
if [ ! -f "$CONFIG_FILE" ]; then
    echo "??  生產環境設定檔不存在: $CONFIG_FILE"
    echo "?? 請從 appsettings.Production.template.json 複製並修改設定值"
    echo "?? 指令: cp appsettings.Production.template.json $CONFIG_FILE"
else
    echo "? 生產環境設定檔已存在"
fi

# 檢查防火牆設定
if command -v ufw &> /dev/null; then
    if ufw status | grep -q "Status: active"; then
        if ! ufw status | grep -q "80/tcp"; then
            echo "??  防火牆未開放 80 埠"
            echo "?? 開放指令: sudo ufw allow 80/tcp"
        else
            echo "? 防火牆已開放 80 埠"
        fi
        
        if ! ufw status | grep -q "443/tcp"; then
            echo "??  防火牆未開放 443 埠"
            echo "?? 開放指令: sudo ufw allow 443/tcp"
        else
            echo "? 防火牆已開放 443 埠"
        fi
    else
        echo "??  防火牆未啟用"
    fi
fi

# 檢查可用磁碟空間
AVAILABLE_SPACE=$(df / | awk 'NR==2 {print $4}')
if [ "$AVAILABLE_SPACE" -lt 2097152 ]; then  # 2GB in KB
    echo "??  可用磁碟空間不足 2GB，當前可用: $(df -h / | awk 'NR==2 {print $4}')"
    echo "?? 請清理磁碟空間或擴展儲存空間"
else
    echo "? 磁碟空間充足: $(df -h / | awk 'NR==2 {print $4}') 可用"
fi

# 檢查記憶體
AVAILABLE_MEMORY=$(free -m | awk 'NR==2{print $7}')
if [ "$AVAILABLE_MEMORY" -lt 512 ]; then
    echo "??  可用記憶體不足 512MB，當前可用: ${AVAILABLE_MEMORY}MB"
    echo "?? 建議至少有 1GB 記憶體用於部署"
else
    echo "? 記憶體充足: ${AVAILABLE_MEMORY}MB 可用"
fi

# 檢查網路連線
if ! curl -s --max-time 10 https://github.com > /dev/null; then
    echo "??  無法連接到 GitHub，請檢查網路連線"
else
    echo "? 網路連線正常"
fi

# 檢查 SSH 金鑰設定（僅供參考）
SSH_DIR="/home/$(whoami)/.ssh"
if [ -d "$SSH_DIR" ]; then
    if [ -f "$SSH_DIR/authorized_keys" ]; then
        echo "? SSH 金鑰設定已存在"
    else
        echo "??  SSH authorized_keys 檔案不存在"
        echo "?? 請確保 GitHub Actions 可以透過 SSH 連接到此伺服器"
    fi
else
    echo "??  SSH 目錄不存在"
    echo "?? 請確保 SSH 已正確設定"
fi

echo ""
echo "?? 部署前檢查完成！"
echo ""
echo "?? 後續步驟："
echo "1. 確保在 GitHub Repository 中設定了以下 Secrets："
echo "   - SERVER_USERNAME: 此伺服器的使用者名稱 ($(whoami))"
echo "   - SERVER_SSH_KEY: SSH 私鑰內容"
echo "   - SERVER_SSH_PORT: SSH 埠號 (如果不是 22)"
echo ""
echo "2. 推送代碼到 main 分支觸發自動部署"
echo ""
echo "3. 部署完成後可透過以下 URL 訪問："
echo "   - 應用程式: http://$(curl -s ifconfig.me || echo 'YOUR_SERVER_IP')"
echo "   - 健康檢查: http://$(curl -s ifconfig.me || echo 'YOUR_SERVER_IP')/health"
echo ""
echo "? 祝您部署順利！"