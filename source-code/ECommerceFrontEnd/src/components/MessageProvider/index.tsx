import React, { useEffect } from "react";
import { message } from "antd";

const MessageProvider: React.FC = () => {
    const [messageApi, contextHolder] = message.useMessage();

    useEffect(() => {
        (window as any).__messageApi = messageApi;
    }, [messageApi]);

    return <>{contextHolder}</>;
};

export default MessageProvider;
