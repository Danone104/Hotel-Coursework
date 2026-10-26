import { useEffect, useState } from 'react'
import styles from  './Bookings.module.css'
import Button from '@/shared/components/Button'
import {Card} from '@/entities/room'
import { roomApi } from '@/shared/api'
const Bookings = ()=>{
    const [rooms, setRooms] = useState([])
    const [page, setPage] = useState(1) 
    const pageSize = 6;
    const [pageCount,setPageCount]= useState(1)

    const handleNextPageClick = ()=>{
        if(page+1<=pageCount) setPage(page+1)
    }
    const handlePrevPageClick = ()=>{
        if(page-1>=1) setPage(page-1)
    }
    useEffect(()=>{
        roomApi.getByPage(page,pageSize)
        .then(setRooms)
        roomApi.getCount().then(count=>setPageCount(count/pageSize))
    },[page])

    return(
     <>
        <div className={styles["cards-container"]}>
            {rooms.map(room=>(
                <Card key={room.roomId} room={room}/>
            ))}
        </div>
        <div className={styles['paggination-container']}>
            <Button className={`${styles['paggination-button']} ${styles['back']}`}>{'<<'}</Button>
            <Button 
            onClick={handlePrevPageClick}
            className={`${styles['paggination-button']} ${page-1<1? styles['is-disabled']:''}`}
            >{'<'}</Button>
            <div className={styles['paggination-page']}>{page}</div>
            <Button 
            className={`${styles['paggination-button']} ${page+1>pageCount? styles['is-disabled']:''}`}
            onClick={handleNextPageClick}
            >{'>'}</Button>
        </div>
    </>
    )
}

export default Bookings