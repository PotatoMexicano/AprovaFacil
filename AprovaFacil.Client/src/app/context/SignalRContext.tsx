import React, { createContext, useContext, useEffect, useState } from 'react';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { useDispatch } from 'react-redux';
import { requestApi } from '../api/requestApiSlice';
import { toast } from 'sonner';
import { notificationApi } from '../api/notificationApiSlice';

interface SignalRContextProps {
  connection: HubConnection | null;
}
  
const SignalRContext = createContext<SignalRContextProps>({ connection: null });

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const dispatch = useDispatch();

  useEffect(() => {
    const connect = async () =>  {
      const newConnection = new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_URL}/notification`)
      .withAutomaticReconnect()
      .build();

      await newConnection.start();
      console.log('Conectado ao servidor SignalR');

      newConnection.on(`UpdateApproved`, () => {
        console.log('Pedido de atualização recebido');  
        dispatch(requestApi.util.invalidateTags(['Approved']));
      });

      newConnection.on(`UpdateRequests`, () => {
        console.log('Pedido de atualização recebido');  
        dispatch(requestApi.util.invalidateTags(['Requests']));
      });

      newConnection.on(`UpdateNotifications`, () => {
        console.log('Pedido de atualização recebido');  
        dispatch(notificationApi.util.invalidateTags(['Notifications']));
      });

      setConnection(newConnection);
    };

    connect();

    return () => {
      connection?.stop();
    };
  }, []);

  return (
    <SignalRContext.Provider value={{ connection }}>
      {children}
    </SignalRContext.Provider>
  );
}

export const useSignalR = () => useContext(SignalRContext);