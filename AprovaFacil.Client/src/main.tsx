import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { Provider } from 'react-redux'
import { store } from './app/store/store'
import { RouterProvider } from 'react-router-dom'
import { router } from './app/routes/routes'
import { SignalRProvider } from './app/context/signalRContext'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Provider store={store}>
      <SignalRProvider>
        <RouterProvider router={router} />
      </SignalRProvider>
    </Provider>
  </StrictMode>
)
