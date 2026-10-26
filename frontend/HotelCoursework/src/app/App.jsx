import MainPage from '@/pages/MainPage'
import RentHistoryPage from '@/pages/RentHistoryPage'
import Router from './routing/Router'
import './styles/styles.css'
import './styles/normalize.css'
import BookingPage from '@/pages/BookingPage'
import LogginingPage from '@/pages/LogginingPage'
import { UserContextProvider } from '../entities/user'
const App=()=> {
  const routes = {
      '/': MainPage,
      '/booking/:id': BookingPage,
      '/loggining':LogginingPage,
      '/rent-history/:id':RentHistoryPage,
      '*' : ()=><div className='error'> 404 Page not found</div>
  }
  return (
    <UserContextProvider>
      <Router routes={routes}></Router>
    </UserContextProvider>
    
  )
}

export default App
