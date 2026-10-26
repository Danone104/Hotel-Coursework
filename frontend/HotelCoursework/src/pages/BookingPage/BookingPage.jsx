import { useContext, useEffect, useState } from "react"
import Header from "@/widgets/Header"
import Footer from "@/widgets/Footer"
import Button from '@/shared/components/Button'
import styles from './BookingPage.module.css'
import DateRangePicker from "@/features/date-pick"
import { roomApi } from "@/shared/api"
import { UserContext } from "@/entities/user"
import BookingForm from "@/features/booking"
import { bookingApi } from "../../shared/api"


const BookingPage = (props)=>{
    const {params}=props
    const [room, setRoom] = useState(null)
    const {user} = useContext(UserContext)
    const [selectedRange, setSelectedRange] = useState(null)
    const [showAlert,setShowAlert] = useState(false)
    const [error,setError] = useState(null)
    const [nightsCount, setNightsCount] = useState(0)
    const [isFormShown, setIsFormShown] = useState(false)
    const [loading, setLoading] = useState(true)
    const handleDateSelect = (range) => {
        setSelectedRange(range)
        setNightsCount(range.nights)
    }
    useEffect(()=>{
        const getRoom = async () => {
            try {
                const roomServer = await roomApi.getById(params.id)
                setRoom(roomServer)
            } catch (error) {
                console.error('Ошибка загрузки комнаты:', error)
            } finally {
                setLoading(false)
            }
        }
        getRoom()   
    }, [params.id])
    const handleClickDatePicker= ()=>{
        setShowAlert(false)
    }
    const handleCloseForm = ()=>{
        setIsFormShown(false)
    }
    const handleButtonClick = ()=>{
        if(!selectedRange){
            alert('Выберите дату заезда и выезда')
            return
        }
        if(!user){
            setIsFormShown(true)
        } else{
            setShowAlert(true)
            bookingApi.bookingByUser(user.userId,room.roomId,selectedRange.startDate,selectedRange.endDate)
        }
    }
    if (loading) {
        return <div className={styles['no-booking-content']}>Загрузка...</div>
    }
    
    if (!room) {
        return <div className={styles['no-booking-content']}>Номер не найден</div>
    }
    return(
       <> 
            <Header/>
                <div className={`page-content ${styles['booking-page-content']}`}>
                    <div className={`${styles['alert']} ${showAlert ? (error? styles['error']:styles['success']): ''}`}
                        >
                            {error? error :'Вы успешно забронировали комнату!'}
                    </div>
                    <div className={styles['booking-content']}>
                        <DateRangePicker
                        roomId={room.roomId}
                        onSelect={handleDateSelect}
                        minDate={new Date()}
                        onClick={handleClickDatePicker}
                        />
                        
                        <div className={styles['booking-description']}>
                            <p className={styles['booking-description__p']}> 
                                <span> Номер</span><span>{room.title}</span>
                            </p>
                            <p className={styles['booking-description__p']}> 
                                <span> Цена за ночь</span><span>{room.price}</span>
                            </p>   
                            <p className={styles['booking-description__p']}> 
                                <span> Ночей</span><span>{nightsCount}</span>
                            </p>   
                            <p className={styles['booking-description__p']}> 
                                <span> Итого</span><span>{room.price*nightsCount}</span>
                            </p>   
                        </div>
                        <Button className={styles['booking-button']} onClick={handleButtonClick}>Забронировать</Button>
                    </div>

                </div>
            <Footer/>
            <BookingForm
            isShown={isFormShown} 
            onClose={handleCloseForm} 
            dates={selectedRange} 
            room={room} 
            setBookingError={setError}
            setShowBookingAlert={setShowAlert}/>
       </>
    )
}
export default BookingPage