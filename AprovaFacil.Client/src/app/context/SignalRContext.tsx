import React, { createContext, useContext, useEffect, useState } from 'react';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { useDispatch } from 'react-redux';
import { requestApi } from '../api/requestApiSlice';
import { toast } from 'sonner';

interface SignalRContextProps {
  connection: HubConnection | null;
}

const toastId = "update-data-received";

const SignalRContext = createContext<SignalRContextProps>({ connection: null });

export const SignalRProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const dispatch = useDispatch();

  useEffect(() => {
    const connect = async () =>  {
      const newConnection = new HubConnectionBuilder()
      .withUrl(`/notification`)
      .withAutomaticReconnect()
      .build();

      await newConnection.start();
      console.log('Conectado ao servidor SignalR');

      newConnection.on(`UpdateRequests`, () => {
        console.log('Pedido de atualização recebido');  
        toast.info("Novas atualizações dispoíveis!", {
          id: toastId,
          duration: 1000,
        });
        dispatch(requestApi.util.invalidateTags(['Requests']));
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