import Bookings from "@/widgets/Bookings"
import Footer from "@/widgets/Footer"
import Header from "@/widgets/Header"
import { UserContextProvider } from "@/entities/user"

const MainPage = ()=>{
    
    return(
        <>
            <UserContextProvider>
                <Header/>
                    <section className="page-content">
                        <Bookings/>
                    </section>
                <Footer/>
            </UserContextProvider>
        </>
    )
}
export default MainPage