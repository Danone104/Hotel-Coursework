import Header from "../../widgets/Header"
import Footer from "../../widgets/Footer"
import {     UserContext, UserContextProvider } from "../../entities/user"
import Button from "../../shared/components/Button/Button"
import styles from './RentHistoryPage.module.css'
import { useContext, useEffect, useState } from "react"
import { loginApi, rentHistoryApi } from "../../shared/api"
import RouterLink from '../../shared/components/RouterLink'
const PAGE_SIZE = 15;

const RentHistoryPage = (props) => {
    const {params}= props
    const [activeTab, setActiveTab] = useState('bookings');
    const [loading, setLoading] = useState(false);
    const {user,setUser} = useContext(UserContext)
    
    const [bookingsData, setBookingsData] = useState([]);
    const [cancelledData, setCancelledData] = useState([]);
    const [rentsData, setRentsData] = useState([]);
    
    const [currentPage, setCurrentPage] = useState({
        bookings: 1,
        cancelled: 1,
        rents: 1
    });
    
    const [totalPages, setTotalPages] = useState({
        bookings: 1,
        cancelled: 1,
        rents: 1
    });
    const userId = params.id; 

    const loadData = async (tab, page) => {
        setLoading(true);
        try {
            let allData = [];
            switch (tab) {
                case 'bookings':
                    allData = await rentHistoryApi.getActiveBookings(userId,page,PAGE_SIZE);
                    break;
                case 'cancelled':
                    allData = await rentHistoryApi.getCancelledBookings(userId,page,PAGE_SIZE);
                    break;
                case 'rents':
                    allData = await rentHistoryApi.getRentsHistory(userId,page,PAGE_SIZE);
                    break;
                default:
                    return;
            }
            
            const startIndex = (page - 1) * PAGE_SIZE;
            const endIndex = startIndex + PAGE_SIZE;
            const paginatedData = allData.slice(startIndex, endIndex);
            
            switch (tab) {
                case 'bookings':
                    setBookingsData(paginatedData);
                    setTotalPages(prev => ({ ...prev, bookings: Math.ceil(allData.length / PAGE_SIZE) }));
                    break;
                case 'cancelled':
                    setCancelledData(paginatedData);
                    setTotalPages(prev => ({ ...prev, cancelled: Math.ceil(allData.length / PAGE_SIZE) }));
                    break;
                case 'rents':
                    setRentsData(paginatedData);
                    setTotalPages(prev => ({ ...prev, rents: Math.ceil(allData.length / PAGE_SIZE) }));
                    break;
            }
        } catch (error) {
            console.error('Ошибка загрузки:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData(activeTab, currentPage[activeTab]);
    }, [activeTab, currentPage[activeTab]]);

    const handlePageChange = (newPage) => {
        if (newPage < 1 || newPage > totalPages[activeTab]) return;
        setCurrentPage(prev => ({ ...prev, [activeTab]: newPage }));
    };

    const renderContent = () => {
        if (loading) {
            return <div className={styles['loading-message']}>Загрузка...</div>;
        }

        let data = [];
        switch (activeTab) {
            case 'bookings':
                data = bookingsData;
                break;
            case 'cancelled':
                data = cancelledData;
                break;
            case 'rents':
                data = rentsData;
                break;
        }

        return (
            <>
                <div className={styles['scrollable-content']}>
                    {data.length > 0 ? (
                        data.map((item) => (
                            <div key={item.id} className={styles['history-item']}>
                                <h3>{item.roomTitle}</h3>
                                <p>Заезд: {item.checkIn}</p>
                                <p>Выезд: {item.checkOut}</p>
                                {item.reasonCancel && (
                                    <p className={styles['cancel-reason']}>Причина отмены: {item.reasonCancel}</p>
                                )}
                                {activeTab === 'rents' && item.price && (
                                    <p className={styles['price']}>Стоимость: {item.price} ₽</p>
                                )}
                                {activeTab === 'bookings' && (
                                    <button 
                                        className={styles['cancel-button']}
                                        onClick={() => handleCancelBooking(item.id)}
                                    >
                                        Отменить бронь
                                    </button>
                                )}
                            </div>
                        ))
                    ) : (
                        <div className={styles['empty-message']}>Нет данных</div>
                    )}
                </div>
                
                {totalPages[activeTab] > 1 && (
                    <div className={styles['pagination']}>
                        <button
                            onClick={() => handlePageChange(currentPage[activeTab] - 1)}
                            disabled={currentPage[activeTab] === 1}
                            className={styles['pagination-button']}
                        >
                            ← Назад
                        </button>
                        
                        <span className={styles['pagination-info']}>
                            Страница {currentPage[activeTab]} из {totalPages[activeTab]}
                        </span>
                        
                        <button
                            onClick={() => handlePageChange(currentPage[activeTab] + 1)}
                            disabled={currentPage[activeTab] === totalPages[activeTab]}
                            className={styles['pagination-button']}
                        >
                            Вперёд →
                        </button>
                    </div>
                )}
            </>
        );
    };

    const handleCancelBooking = async (bookingId) => {
        if (window.confirm('Вы уверены, что хотите отменить бронирование?')) {
            try {
                await rentHistoryApi.cancelBooking(userId, bookingId);
                alert('Бронирование успешно отменено');
                loadData(activeTab, currentPage[activeTab]);
            } catch (error) {
                alert('Ошибка при отмене бронирования');
            }
        }
    };

    const handleTabClick = (tabId) => {
        setActiveTab(tabId);
        setCurrentPage(prev => ({ ...prev, [tabId]: 1 }));
    };
    const handleLogout = ()=>{
        loginApi.logout()
        setUser(null)
    }
    if(!user|| user.id!=params.id){
        return(<div className={styles["no-authorized"]}>Нет доступа</div>)
    }
    return (
        <>
            <UserContextProvider>
                <Header/>
                <section className={`page-content ${styles['rent-history-content']}`}>
                    <div className={styles['rent-history-container']}>
                        <div className={styles['navigation-wrapper']}>
                            <RouterLink to='/'>
                                <Button onClick={handleLogout} className={styles['logout-button']}>Выйти</Button>
                            </RouterLink>
                            
                            <div className={styles['tabs-navigation']}>
                                <span
                                    onClick={() => handleTabClick('bookings')}
                                    className={`${styles['tab-button']} ${activeTab === 'bookings' ? styles['is-active'] : ''}`}
                                >
                                    Активные брони
                                </span>
                                <span
                                    onClick={() => handleTabClick('cancelled')}
                                    className={`${styles['tab-button']} ${activeTab === 'cancelled' ? styles['is-active'] : ''}`}
                                >
                                    Отменённые
                                </span>
                                <span
                                    onClick={() => handleTabClick('rents')}
                                    className={`${styles['tab-button']} ${activeTab === 'rents' ? styles['is-active'] : ''}`}
                                >
                                    История аренд
                                </span>
                            </div>
                        </div>
                        {renderContent()}
                    </div>
                </section>
                <Footer/>
            </UserContextProvider>
        </>
    );
};

export default RentHistoryPage;