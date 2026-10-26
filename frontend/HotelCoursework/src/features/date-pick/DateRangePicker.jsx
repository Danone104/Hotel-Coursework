// features/booking/ui/DateRangePicker/DateRangePicker.jsx
import { useState, useEffect, useRef, useContext } from 'react'
import styles from './DateRangePicker.module.css'
import { bookingApi } from '@/shared/api'
import { UserContext } from '../../entities/user'
const DateRangePicker = (props) => {
    const {
        roomId, 
        onSelect,              
        minDate = new Date(),  
        maxDate = null,
        onClick       
    }=props
    const [isOpen, setIsOpen] = useState(false)
    const [currentMonth, setCurrentMonth] = useState(new Date())
    const [bookedDates,setBookedDates] = useState([])
    const [startDate, setStartDate] = useState(null)
    const [endDate, setEndDate] = useState(null)
    const [hoverDate, setHoverDate] = useState(null)
    const [error, setError] = useState('')
    const pickerRef = useRef(null)
    const {user} = useContext(UserContext)
    
    useEffect(() => {
        const handleClickOutside = (event) => {
            if (pickerRef.current && !pickerRef.current.contains(event.target)) {
                setIsOpen(false)
            }
        }
        document.addEventListener('mousedown', handleClickOutside)
        return () => document.removeEventListener('mousedown', handleClickOutside)
    }, [])
    
    const formatDate = (date) => {
        if (!date) return ''
        return date.toLocaleDateString('ru-RU', {
            day: 'numeric',
            month: 'numeric',
            year: 'numeric'
        })
    }
    const isDateBooked = (date) => {
        const dateStr = date.toISOString().split('T')[0]
        return bookedDates.includes(dateStr)
    }
    
    const hasBookedDatesInRange = (start, end) => {
        const current = new Date(start)
        const endDate = new Date(end)
        
        while (current <= endDate) {
            if (isDateBooked(current)) {
                return true
            }
            current.setDate(current.getDate() + 1)
        }
        return false
    }
    
    const handleDateClick = (date) => {
        if (isDateBooked(date)) {
            setError('Эта дата уже занята. Выберите другую дату.')
            setTimeout(() => setError(''), 3000)
            return
        }
        
        if (!startDate || (startDate && endDate)) {
            setStartDate(date)
            setEndDate(null)
            setError('')
        } 
        else if (startDate && !endDate) {
            let newStart = startDate
            let newEnd = date
            
            if (date < startDate) {
                newStart = date
                newEnd = startDate
            }
            if (hasBookedDatesInRange(newStart, newEnd)) {
                setError('Выбранный диапазон содержит занятые даты. Пожалуйста, выберите другие даты.')
                setTimeout(() => setError(''), 3000)
                return
            }
            
            setStartDate(newStart)
            setEndDate(newEnd)
            setError('')
            
            if (onSelect) {
                onSelect({
                    startDate: newStart,
                    endDate: newEnd,
                    startDateStr: newStart.toISOString().split('T')[0],
                    endDateStr: newEnd.toISOString().split('T')[0],
                    nights: Math.ceil((newEnd - newStart) / (1000 * 60 * 60 * 24))
                })
            }
        }
    }
    
    const isInRange = (date) => {
        if (!startDate) return false
        if (startDate && endDate) {
            return date >= startDate && date <= endDate
        }
        if (startDate && hoverDate && !endDate) {
            const start = startDate
            const end = hoverDate
            const rangeStart = start < end ? start : end
            const rangeEnd = start < end ? end : start
            return date >= rangeStart && date <= rangeEnd
        }
        return false
    }
    
    const isRangeEdge = (date) => {
        return (startDate && date.toDateString() === startDate.toDateString()) ||
               (endDate && date.toDateString() === endDate.toDateString())
    }
    const getMonthName = (date) => {
        return date.toLocaleDateString('ru-RU', { month: 'long', year: 'numeric' })
    }
    
    const prevMonth = () => {
        setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() - 1, 1))
    }
    
    const nextMonth = () => {
        if (maxDate) {
            const nextMonthDate = new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1, 1)
            if (nextMonthDate <= maxDate) {
                setCurrentMonth(nextMonthDate)
            }
        } else {
            setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1, 1))
        }
    }
    
    const generateDays = () => {
        const year = currentMonth.getFullYear()
        const month = currentMonth.getMonth()
        
        const firstDayOfMonth = new Date(year, month, 1)
        const lastDayOfMonth = new Date(year, month + 1, 0)
        
        const startDayOfWeek = firstDayOfMonth.getDay() || 7 
        const days = []
        
        for (let i = 1; i < startDayOfWeek; i++) {
            days.push(null)
        }
        
        for (let d = 1; d <= lastDayOfMonth.getDate(); d++) {
            const date = new Date(year, month, d)
            const isBooked = isDateBooked(date)
            const isDisabled = isBooked || (minDate && date < new Date(minDate.setHours(0,0,0,0)))
            
            days.push({
                date,
                day: d,
                isBooked,
                isDisabled
            })
        }
        
        return days
        
    }
    
    const days = generateDays()
    const weekDays = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс']
    

    useEffect(()=>{
        const getDates=async ()=>{

            const occupiedDates= await bookingApi.getOccupiedDates(
                roomId,
                currentMonth.getFullYear(),
                currentMonth.getMonth()+1,
                user
            )
            setBookedDates(occupiedDates)
            

        }
        getDates()
    },[currentMonth])
    
    return (
        <div onClick={onClick} className={styles.dateRangePicker} ref={pickerRef}>
            <div 
                className={styles.datePickerInput}
                onClick={() => setIsOpen(!isOpen)}
            >
                <span className={styles.dateRangeText}>
                    {startDate ? formatDate(startDate) : 'Дата заезда'}
                </span>
                <span className={styles.dateSeparator}>—</span>
                <span className={styles.dateRangeText}>
                    {endDate ? formatDate(endDate) : 'Дата выезда'}
                </span>
            </div>
            
            {isOpen && (
                <div className={styles.calendar}>
                    {error && (
                        <div className={styles.errorMessage}>
                            ⚠️ {error}
                        </div>
                    )}
                    
                    <div className={styles.calendarHeader}>
                        <button onClick={prevMonth} className={styles.navButton}>←</button>
                        <span className={styles.monthName}>{getMonthName(currentMonth)}</span>
                        <button onClick={nextMonth} className={styles.navButton}>→</button>
                    </div>
                    
                    <div className={styles.weekDays}>
                        {weekDays.map(day => (
                            <div key={day} className={styles.weekDay}>{day}</div>
                        ))}
                    </div>
                    
                    <div className={styles.days}>
                        {days.map((day, index) => {
                            if (!day) {
                                return <div key={`empty-${index}`} className={styles.emptyDay}></div>
                            }
                            
                            const inRange = isInRange(day.date)
                            const isEdge = isRangeEdge(day.date)
                            const isSelected = (startDate && day.date.toDateString() === startDate.toDateString()) ||
                                             (endDate && day.date.toDateString() === endDate.toDateString())
                            
                            return (
                                <div
                                    key={day.day}
                                    className={`
                                        ${styles.day}
                                        ${day.isDisabled ? styles.disabled : ''}
                                        ${day.isBooked ? styles.booked : ''}
                                        ${inRange ? styles.inRange : ''}
                                        ${isEdge ? styles.rangeEdge : ''}
                                        ${isSelected ? styles.selected : ''}
                                    `}
                                    onClick={() => !day.isDisabled && handleDateClick(day.date)}
                                    onMouseEnter={() => !day.isDisabled && !endDate && startDate && setHoverDate(day.date)}
                                    onMouseLeave={() => setHoverDate(null)}
                                >
                                    {day.day}
                                    {day.isBooked && <span className={styles.bookedDot}>●</span>}
                                </div>
                            )
                        })}
                    </div>
                    
                    <div className={styles.legend}>
                        <div className={styles.legendItem}>
                            <div className={`${styles.legendColor} ${styles.legendAvailable}`}></div>
                            <span>Свободно</span>
                        </div>
                        <div className={styles.legendItem}>
                            <div className={`${styles.legendColor} ${styles.legendSelected}`}></div>
                            <span>Выбрано</span>
                        </div>
                        <div className={styles.legendItem}>
                            <div className={`${styles.legendColor} ${styles.legendBooked}`}></div>
                            <span>Занято</span>
                        </div>
                    </div>
                </div>
            )}
        </div>
    )
}

export default DateRangePicker